#pragma once
#include "Arduino.h"

#define MMDC_ZERO_LEFT   B00
#define MMDC_ZERO_RIGHT  B10
#define MMDC_ZERO_TOP    B00
#define MMDC_ZERO_BOTTOM B01

#define MMDC_ZERO_LT MMDC_ZERO_LEFT | MMDC_ZERO_TOP
#define MMDC_ZERO_LB MMDC_ZERO_LEFT | MMDC_ZERO_BOTTOM
#define MMDC_ZERO_RT MMDC_ZERO_RIGHT | MMDC_ZERO_TOP
#define MMDC_ZERO_RB MMDC_ZERO_RIGHT | MMDC_ZERO_BOTTOM

#define MMDC_GEOMETRY_HORIZONTAL 0
#define MMDC_GEOMETRY_VERTICAL   1

#define MMDC_SEQUENCE_SNAKE 0
#define MMDC_SEQUENCE_COMB  1

class Matrix {
private:
	int _width, _height, _numleds;
	int8_t _zero, _geometry, _sequence;

public:
	Matrix(int width, int height, int8_t zero, int8_t geometry, int8_t sequence);
	int getIndex(int x, int y);
};

