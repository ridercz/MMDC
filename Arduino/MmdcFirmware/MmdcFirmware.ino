#include <FastLED.h>
#include <SerialNumber.h>
#include "Matrix.h"

#define SERIAL_SPEED  115200
#define VERSION       "NeoPixel-Display/1.0.0"
#define WIDTH         16
#define HEIGHT        16
#define SN_LENGTH     8
#define ZERO          MMDC_ZERO_LT
#define GEOMETRY      MMDC_GEOMETRY_VERTICAL
#define SEQUENCE      MMDC_SEQUENCE_SNAKE
#define LED_CHIPSET   WS2811
#define LED_PIN       3
#define LED_ORDER     GRB
#define ACK_CHAR      0x06
#define INIT_DELAY    250

Matrix m = Matrix(WIDTH, HEIGHT, ZERO, GEOMETRY, SEQUENCE);
CRGB leds[WIDTH * HEIGHT];
int currentX, currentY;

void setup() {
  // Init LEDs and turn them off
  FastLED.addLeds<LED_CHIPSET, LED_PIN, LED_ORDER>(leds, WIDTH * HEIGHT);

  // Enable serial port
  Serial.begin(SERIAL_SPEED);

  // Send signature
  FastLED.showColor(CRGB(0x33, 0x00, 0x00));
  Serial.println("MMDC Display Connected");
  Serial.print("VERSION=");
  Serial.println(VERSION);
  delay(INIT_DELAY);

  // Send dimentsions
  Serial.print("WIDTH=");
  Serial.println(WIDTH);
  FastLED.showColor(CRGB(0x00, 0x33, 0x00));
  delay(INIT_DELAY);
  Serial.print("HEIGHT=");
  Serial.println(HEIGHT);
  FastLED.showColor(CRGB(0x00, 0x00, 0x33));
  delay(INIT_DELAY);

  // Send serial number
  FastLED.showColor(CRGB(0x33, 0x33, 0x33));
  sendOrGetSerialNumber();
  delay(INIT_DELAY);

  // Send ready signal
  Serial.println("OK");
  FastLED.showColor(CRGB::Black);
  
  // Set current coordinates
  currentX = 0;
  currentY = 0;
}

void loop() {
  if (Serial.available() < 3) return;

  // Read RGB values from serial
  unsigned char buffer[3];
  Serial.readBytes(buffer, 3);

  // Write to right LED
  leds[m.getIndex(currentX, currentY)] = CRGB(buffer[0], buffer[1], buffer[2]);
  currentX++;

  if (currentX == WIDTH) {
    // Next line
    currentX = 0;
    currentY++;
  }

  if (currentY == HEIGHT) {
    // Next frame
    currentY = 0;
    FastLED.show();
    Serial.write(ACK_CHAR);
  }

}

void sendOrGetSerialNumber() {
  // Prepare the Serial Number class
  SerialNumber snUtil = SerialNumber(SN_LENGTH);

  // Prepare array to hold the serial number
  unsigned char sn[SN_LENGTH];

  if (!snUtil.hasSerialNumber()) {
    // No serial number - read it from serial port
    Serial.println("SN?");
    while (Serial.available() < SN_LENGTH);
    Serial.readBytes(sn, SN_LENGTH);

    // Save the values as serial number
    snUtil.setSerialNumber(sn);
    Serial.println();
  }

  // Print serial number in hex
  Serial.print("SN=");
  snUtil.getSerialNumber(sn);
  for (int i = 0; i < SN_LENGTH; i++) {
    if (sn[i] < 0x10) Serial.print("0");
    Serial.print(sn[i], HEX);
  }
  Serial.println();
}
