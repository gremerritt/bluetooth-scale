# bluetooth-scale

## Description
My project was to create a Bluetooth-enabled scale. The motivation behind the project is that it’s often difficult to remember what items you need to pick up when you go to the grocery store. The idea is to create a system where the data from the scale is available remotely. For example, a user could keep a gallon of milk on the scale. Then when they’re at the store the user would be able to check the weight on the scale and see if they need to pick up more! The scope of this project is bit a bit smaller than this general idea. In my project I created a scale that is Bluetooth enabled, and can send the scale data to a PC that has a Bluetooth connection.

My general design is fairly straightforward. The first component is the weight scale. The voltage at the output of the scale varies according to the force applied to the scale. The MSP430 microcontroller uses a GPIO pin to read the voltage at the output of the scale. This pin is then used by the MSP430’s internal analog to digital converter to read in the voltage level from the scale. This value is then sent to a connected Bluetooth module. Finally the Bluetooth module transmits the data it receives from the MSP430. Any device connected to the Bluetooth module can then read the transmitted data.

To create a scale, I purchase a load cell from SparkFun (found here: https://www.sparkfun.com/products/13329). In working with the load cell, I realized that it was surprisingly difficult to get a good signal from it. The load cell works by using a Wheatstone bridge. As different parts of the load cell are stressed the resistance in different legs of the Wheatstone bridge changes, which changes the voltage at the output terminals of the bridge. The changes in resistance are extremely small, so the way to use the load cell is to amplify the difference between its positive and negative outputs. Try as I might, I wasn’t able to achieve this amplification in a way that would usable for the rest of my design. With a 10-bit ADC value and a reference voltage between 3.3V and 0V, I could get down to a resolution of a few millivolts, but the load cell voltage still wasn’t reliable enough.

With my load cell not working, I decided to go with a plan B. Often electronic parts are shipped embedded in conductive foam. I’d read before that a piece of conductive foam could be used to create a pressure sensor, simply by inserting two wires into the foam. The foam conducts, and so acts as a resistor. As more pressure is applied, the foam compresses and the resistance of the foam decreases. I placed two wires in my piece of foam (which I happened to have lying around), and glued it to a small wooden board, which would act as the base of my scale. When uncompressed the resistance of the foam was around 500kOhm, and when very compressed the resistance was around 36kOhm. I created a voltage divider with the variable resistance foam and a 30kOhm resistor, which gave me an output voltage range of ~0.18V - 1.5V. To complete the scale I added another flat board on top of the board that I had glued the pressure sensor onto. Thus, when an object is placed on top of the board it “sandwiches” the pressure sensor between the boards, and allows for fairly reliable data.
![alt text](https://github.com/gremerritt/bluetooth-scale/blob/master/images/pressure_sensor.png "Pressure Sensor From Resistive Foam")

Now that my pressure sensor was working, I got to work on the MSP430. In my main() function I set the clock to run at 1MHz, and initialized the watchdog timer so that it wakes up the CPU and runs its handler every ~8ms. I then initialized the ADC. During the initialization the input is set for pin P1.4. The reference voltage is set for the default 3.3V, the ADC is turned on, and interrupts are enabled. Next, I initialized the components for UART communication, which I used to communicate with the Bluetooth module. The SMCLK rate and buadrate are set, the RX and TX pins are configured on P1.0 and P1.1, respectively (although RX is never used), and UART state machine is enabled. Finally interrupts are enabled and the CPU is put into low power mode.


There is a general counter, WDT_counter, which keeps track of the number of watchdog interrupts. Every 5 interrupts, the watchdog timer handler kicks off an ADC conversion by setting a bit in the ADC10CTL0 register. The WDT_counter counter is then reset. When the ADC conversion has completed, the ADC completion handler is called. In order to smooth out some of the noise from the weight sensor, I keep track of the average of the last 50 ADC readings. To do this, I have an array of 50 numbers. Each time a new ADC value is available, the current value in the array are shifted back by one (so that element 0 disappears, element 1 becomes 0, 2 becomes 1, etc. The new value is then added as the last element in the array. There is another global variable counter, send_counter, which tracks how many ADC conversions we’ve done since a value was send out over the UART transmission (TX) line. After every 50 ADC conversions (meaning we have a whole new set of data from the ADC readings) the average ADC reading value is set out.
 
To do the UART communication there is a function called tx_start(), which takes as input a pointer to a c-string and the length of the string. This function sets tx_next_character to the pointer to the c-string and tx_count to the length of the string. It then enables the transmit interrupt. This interrupt essentially gets called immediately. The interrupt handler is called once for each character in the c-string (tx_count times.) Each time the interrupt is called, the next character in the c-string is written to the UCA0TXBUF register. This value is sent out over the UART transmission line.

The next step was to get the Bluetooth module working. The device I used was the RN42 module (purchased from SparkFun, here: https://www.sparkfun.com/products/12574). The first challenge came in even getting the hardware connected. The chip didn’t have any pins that could be plugged into a breadboard, so I needed to solder wires onto the chip. The leads on the chip were extremely small, but after a lot of work I managed to get wires soldered onto the Vcc, GND, and UART RX leads. I also soldered wires onto the SPI leads (SIMO, MISO, and SCLK) but UART ended up being much easier to work with, so those leads were unconnected. From here I was able to power up the chip!

Once it was powered up the Bluetooth module appeared on my list of available Bluetooth devices on my PC, and I was able to pair my computer up with the device.
![alt text](https://github.com/gremerritt/bluetooth-scale/blob/master/images/manage_bluetooth_devices.png "Connecting to the Bluetooth Device")

By looking in my device manager, I could see that the Bluetooth module had a connection open on one of my PCs COM ports (the port changed each time the device was unpaired/re-paired.) From here I opened up the PuTTY application and opened up a serial connection on the COM port. The Bluetooth module included a guide on how to work with the module (which can be found here: https://www.sparkfun.com/datasheets/Wireless/Bluetooth/rn-bluetooth-um.pdf). In order to enter command-mode, I typed ‘$$$’ and then received ‘CMD’ back from the RN42. Success! The first thing I did was look at the current settings of the device. The screenshot below shows what the settings looks like in PuTTY (this is from after some settings were changed.)
![alt text](https://github.com/gremerritt/bluetooth-scale/blob/master/images/putty_connected_to_rn42.png "Connecting to the Bluetooth Device Via PuTTY")

First I changed the baudrate on the Bluetooth module to 9600, which I was able to do by typing ‘SU,96’. I exited from command-mode and was, at least in theory, ready to start receiving data from the RN42. I edited the MSP430 code so that it would send out the byte value 0x41 (‘A’). Unfortunately, no data was received on serial connection on the PC. After a fair amount of digging I found out that the default configuration of the RN42 was to require authentication (with default PIN of ‘1234’). This would prevent the module from accepting the data that I was sending it from the MSP430. Fortunately, the authentication settings are fully configurable! I jumped back into command-mode from the serial connection on my PC, and turned authentication off by sending ‘SA,0’ to the RN42. I retested with the MSP430 sending ‘A’ and was now receiving something on the PC, but unfortunately it was not the letter ‘A’ that I was trying to send (what I was actually receiving was hex 0x80.)

After a fair amount more research, I realized that the clock rate of the RN42 was 1MHz. However, the UART configuration on the MSP430 was set up to run at 8MHz. I changed the configuration on the MSP430 so that the UART was set up for 1MHz, and (voila!) I was able to receive data on the PC that was sent from the MSP430 to the RN42. I edited the code on the MSP430 so that it was sending the actual ADC values from the pressure sensor. At this point all of the major elements of my project were working. I could press on the scale, and see the values being received on the PC change according to the pressure on the scale.

My last step was to create a simple C#/.NET GUI application that would display the weight on the scale. The application provides a way to scan for open COM ports and connect to the port. The application then reads from the COM port, and translates the ADC value (which is what is sent over Bluetooth by the RN42) to a value displayed in pounds. To do the conversion from the ADC value to pounds, I did some experimentation on the scale with known weights (I used water – 1 gallon of water weights around 8.3 pounds.) With no weight the scale produces an ADC value of about 250, and with 8.3 pounds, the scale produces an ADC value of about 500. The ADC value is also roughly linear with the weight. With this data, I was able to come up with an equation to convert the ADC value to a weight value. For reference, the GUI application also displays the raw ADC value received.
![alt text](https://github.com/gremerritt/bluetooth-scale/blob/master/images/windows_gui_app.png "Windows GUI Application")

I think that overall the project was a success. Although the scale isn’t terribly accurate it would serve the need of being able to tell roughly the amount of weight on the scale, especially when that weight is in the range of ~3 to ~10 pounds. In my opinion the largest success of this project was getting the Bluetooth connection working. It’s really neat to be able to send data wirelessly from the MSP430 (or any microcontroller) to a PC. I actually really enjoyed going through the process of learning about a new piece of technology (the RN42) and getting more specific knowledge about how Bluetooth in general (and the RN42) works. I could see myself working on other projects in the future where I would quickly be able to add a Bluetooth/wireless connection element.

My final schematic and images of the project are shown below:
![alt text](https://github.com/gremerritt/bluetooth-scale/blob/master/images/schematic.png "Hardware Schematic")
![alt text](https://github.com/gremerritt/bluetooth-scale/blob/master/images/breadboard.png "Breadboard Layout")
![alt text](https://github.com/gremerritt/bluetooth-scale/blob/master/images/full_project.png "Full Project Layout")

## Next Steps
The most important change that I would make to my design is to increase the accuracy and consistency of the scale. There are a couple different levels at which I could accomplish this. A small step in the right direction would be to replace the piece of conductive foam that is acting as the variable resistor that makes the scale work. In my design I super glued the piece of foam onto a board in an attempt to make the scale easier to work with. However, in doing so the foam sucked up a lot of the glue, which made the foam less "squishy" (technical term) and made the resistance of the foam less reliable. However, with only one piece of the foam on hand I went ahead and used this piece (which is still reasonably accurate).

A larger step in the right direction would be to use an actual load cell to translate the weight on the scale to a voltage that could be used by the ADC. A load cell works by using a Wheatstone bridge. However the change in resistance in legs of the Wheatstone bridge are very small, so the way to use it is to (heavily) amplify the difference between the positive and negative voltages from the bridge. Unfortunately I had trouble getting the amplification on the load cell I had purchased to work, and thus wasn't able to get usable signal from the load cell. In theory however, a functional load cell should be able to give reliable results down to a few grams. This would make the scale much more useful!

Finally, I would also add a web element to the project. My original idea was to create a smart sensor that someone could use to keep track of how much of something they had in. For example, you could keep a gallon of milk on the scale. The sensor would then send the weight data to a web server, which someone could check while they were at the store. This wouldn't be too hard to implement, as I already have a simple C# application that's reading the weight data from the Bluetooth module. This same application could send the data up to a server, which could be accessed from anywhere!
