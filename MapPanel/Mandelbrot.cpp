int calcPoint(double x0, double y0, int maxIterations) {

	double x = x0;
	double y = y0;
	double xSucc;
	double ySucc;

	for(int i = 1; i < maxIterations; ++i) {
		xSucc = x * x - y * y + x0;
		ySucc = 2.0 * x * y + y0;
		x = xSucc;
		y = ySucc;

		if((x >= 10.0) || (y >= 10.0))
			return i;
	}
	return 0;
}
