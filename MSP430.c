#include "msp430g2553.h"
#include "string.h"
#include "stdio.h"

#define ADC_INPUT_BIT_MASK 0x10
#define ADC_INCH INCH_4
#define ARRAY_SIZE 50
#define WDT_INTERVAL 5

void init_wdt_no_interrupt(void);
void init_adc(void);
void init_USCI_UART(void); // sets up the UART
int tx_start(char*, int);// starts transmission of a buffer

volatile unsigned int input_readings[ARRAY_SIZE];
unsigned int input_avg = 0;
char * to_send;
unsigned int send_counter = ARRAY_SIZE;
unsigned int WDT_counter = WDT_INTERVAL;

void main(){
	WDTCTL = WDTPW + WDTHOLD; // Stop watchdog timer
	BCSCTL1 = CALBC1_1MHZ; // 1Mhz calibration for clock
	DCOCTL = CALDCO_1MHZ;

	init_adc();
	init_wdt_no_interrupt(); // does NOT enable the wdt interrupt yet
	init_USCI_UART(); // initialize the UART

	IE1 |= WDTIE; // enable WDT interrupt so that conversions can start
	_bis_SR_register(GIE+LPM0_bits); //powerdown CPU
 }

void init_wdt_no_interrupt(){
	// setup the watchdog timer as an interval timer
	// INTERRUPT NOT YET ENABLED!
	WDTCTL =(WDTPW +	// (bits 15-8) password
						// bit 7=0 => watchdog timer on
						// bit 6=0 => NMI on rising edge (not used here)
						// bit 5=0 => RST/NMI pin does a reset (not used here)
			WDTTMSEL +	// (bit 4) select interval timer mode
			WDTCNTCL +	// (bit 3) clear watchdog timer counter
			0 +			// bit 2=0 => SMCLK is the source
			1			// bits 1-0 = 01 => source/8K
	);
}

// Initialization of the ADC
void init_adc(){
	ADC10CTL1 = ADC_INCH			//input channel 4
				+SHS_0				//use ADC10SC bit to trigger sampling
				+ADC10DIV_4			// ADC10 clock/5
				+ADC10SSEL_0		// Clock Source=ADC10OSC
				+CONSEQ_0;			// single channel, single conversion

	ADC10AE0 = ADC_INPUT_BIT_MASK;	// enable A4 analog input
	ADC10CTL0 = SREF_0				//reference voltages are Vss and Vcc
				+ADC10SHT_3			//64 ADC10 Clocks for sample and hold time (slowest)
				+ADC10ON			//turn on ADC10
				+ENC				//enable (but not yet start) conversions
				+ADC10IE;			//enable interrupts
}

#define CBUFLEN 20
char cbuffer[CBUFLEN]; // buffer for output of characters
char tmp = 0;
void interrupt adc_handler(){
	unsigned int tmp_avg = 0;
	unsigned char i;
	for (i=1; i<ARRAY_SIZE; i++)
	{
		tmp_avg += input_readings[i];
		input_readings[i-1] = input_readings[i];
	}
	input_readings[ARRAY_SIZE - 1] = ADC10MEM;
	tmp_avg += input_readings[ARRAY_SIZE - 1];

	input_avg = tmp_avg / ARRAY_SIZE;
	if (--send_counter == 0)
	{
		snprintf(cbuffer, CBUFLEN, "%d\r\n", input_avg);
		tmp++;
		tx_start(cbuffer, strlen(cbuffer));
		send_counter = ARRAY_SIZE;
	}

}
ISR_VECTOR(adc_handler, ".int05")

// ===== Watchdog Timer Interrupt Handler =====
interrupt void WDT_interval_handler(){
	if (--WDT_counter == 0)
		{
			ADC10CTL0 |= ADC10SC;	// kick off a conversion
			WDT_counter = WDT_INTERVAL;
		}
}
ISR_VECTOR(WDT_interval_handler, ".int10")

/*
 * UART Timing Parameters
 * The UART will be driven by the SMCLK at 1Mhz
 * The baudrate is 9600
 */
#define SMCLKRATE 1000000
#define BAUDRATE 9600

#define BRDIV16 ((16*SMCLKRATE)/BAUDRATE)
#define BRDIV (BRDIV16/16)
#define BRMOD ((BRDIV16-(16*BRDIV)+1)/16)
#define BRDIVHI (BRDIV/256)
#define BRDIVLO (BRDIV-BRDIVHI*256)

//Port 1 pins used for transmit and receive are P1.2 and P1.1
#define TXBIT 0x04
#define RXBIT 0x02

void init_USCI_UART(){
	UCA0CTL1 = UCSWRST;   // reset and hold UART state machine
	UCA0CTL1 |= UCSSEL_2; // select SMCLK as the clock source
	UCA0BR1=BRDIVHI;      // set baud parameters, prescaler hi
	UCA0BR0=BRDIVLO;	  // prescaler lo
	UCA0MCTL=2*BRMOD;     // modulation
	// setup the TX pin (connect the P1.2 pin to the USCI)
	P1SEL |= TXBIT;
	P1SEL2|= TXBIT;
	UCA0CTL1 &= ~UCSWRST; // allow the UART state machine to operate
}

char *tx_next_character;  // pointer to the next character to transmit
int tx_count;             // remaining number of characters to transmit

void interrupt tx_handler(){
	if (tx_count>0){ // are there characters left to transmit?
		--tx_count;  // decrement the count
		UCA0TXBUF = *tx_next_character++; // send the current character & advance the pointer
	} else {         // when no characters left
		IE2 &= ~UCA0TXIE; // disable the transmit interrupt
	}
}
ISR_VECTOR(tx_handler,".int06") // declare interrupt vector

int tx_start(char *c_string, int count){
	if(tx_count==0){          // check that a previous transmission is not still in progress
		tx_count=count;       // store parameters for the transmit interrupt
		tx_next_character=c_string;
		IE2  |= UCA0TXIE;     // enable the transmit interrupt (this will immediately generate the 1st interrupt)
		return 0;             // success (transmission started)
	} else {                  // busy error
		return -1;            // failure
	}
}
