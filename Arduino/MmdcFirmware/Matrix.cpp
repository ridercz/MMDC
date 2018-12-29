#include "Arduino.h"
#include "Matrix.h"

Matrix::Matrix(int width, int height, int8_t zero, int8_t geometry, int8_t sequence) {
	_width = width;
	_height = height;
	_zero = zero;
	_geometry = geometry;
	_sequence = sequence;
}

int Matrix::getIndex(int x, int y) {
	// Validate if index is inside bounds of array
	if (x < 0 || x >= _width || y < 0 || y >= _height) return -1;

	// Subtract coordinates for non-standard zero position
	if (_zero & MMDC_ZERO_RIGHT) x = _width - x - 1;
	if (_zero & MMDC_ZERO_BOTTOM) y = _height - y - 1;

	// Swap coordinates for vertical geometry
	int xi = _geometry == MMDC_GEOMETRY_VERTICAL ? y : x;
	int yi = _geometry == MMDC_GEOMETRY_VERTICAL ? x : y;
	uint8_t size = _geometry == MMDC_GEOMETRY_VERTICAL ? _height : _width;

	if (_sequence == MMDC_SEQUENCE_COMB) {
		return yi * size + xi;
	}
	else { // MMDC_SEQUENCE_SNAKE
		return yi * size + ((yi & 0x01) ? (size - xi - 1) : xi);
	}
}


