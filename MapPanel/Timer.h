#ifndef _TIMER_H
#define _TIMER_H

#include <iostream>
#include <windows.h>
#include <cmath>


// only for windows!

class Timer {
public:
	LONGLONG freq;
	LONGLONG time1;
	LONGLONG time2;

public:
	Timer() {
		QueryPerformanceFrequency((LARGE_INTEGER*)&freq);
	}

	void start() {
		QueryPerformanceCounter((LARGE_INTEGER*)&time1);
	}

	void stop() {
		QueryPerformanceCounter((LARGE_INTEGER*)&time2);
	}

	double diffTime() {
		return (((double)(time2-time1))/((double)freq));
	}

	int sec() {
		double secs;
		modf(diffTime(), &secs);
		return (int)secs;
	}

	int msec() {
		double secs;
		return (int)(modf(diffTime(), &secs) * 1000.0f) % 1000;
	}

	int usec() {
		double secs;
		return (int)(modf(diffTime(), &secs) * 1000000.0f) % 1000;
	}

	void printDifference(char* what = "Time taken") {
		std::cout << what << " : " << diffTime() << " s" << std::endl;
	}
};
#endif
