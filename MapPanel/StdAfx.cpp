// stdafx.cpp : source file that includes just the standard includes
//	MapPanel.pch will be the pre-compiled header
//	stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"
#include <wchar.h>
#include <sstream>

// TODO: reference any additional headers you need in STDAFX.H
// and not in this file

//void MPGetFileName(const wchar_t * filename_in, wchar_t * filename_out) {
//	int pos1 = 0;
//	int i;
//	int len_in = wcslen(filename_in);
//	int pos2 = len_in - 1;
//
//	filename_out = (wchar_t *) malloc(sizeof(wchar_t) * len_in);
//
//	
//
//	//// find the position of the last "\" and "." character in filename
//	//for (i = 0; i < len_in; ++i) {
//	//	if (filename_in[i] == L"\\") pos1 = i;
//	//	else if (filename_in[i] == L".") pos2 = i;
//	//}
//
//	//// get filename without extension
//	//int len_out = pos2 - pos1;
//	//filename_out = (wchar_t *) malloc(sizeof(wchar_t) * len_out);
//
//	//if (filename_out) {
//	//	for (i = 0; i < len_out; ++i) {
//	//		filename_out[i] = filename_in[i];
//	//	}
//	//}
//}

void MPGetFileName(const char * filename_in, char** filename_out) {

	if (!filename_out) return;

	int pos1 = 0;
	int i;
	int len_in = strlen(filename_in);
	int pos2 = len_in - 1;

	// find the position of the last "\" and "." character in filename
	for (i = 0; i < len_in; ++i) {
		if (filename_in[i] == '\\') pos1 = i + 1;
		else if (filename_in[i] == '.') pos2 = i;
	}

	// get filename without extension
	int len_out = pos2 - pos1;
	char* temp = (char *) malloc(sizeof(char) * len_out + 1);

	if (temp) {
		for (i = 0; i < len_out; ++i) {
			temp[i] = filename_in[pos1 + i];
		}
		temp[len_out] = '\0';
	}

	*filename_out = temp;
}

//    
//    
//    Dim posn As Integer, i As Integer
//    Dim fName As String
//    
//    posn = 0
//    'find the position of the last "\" character in filename
//    For i = 1 To Len(flname)
//        If (Mid(flname, i, 1) = "\") Then posn = i
//    Next i
//
//    'get filename without path
//    fName = Right(flname, Len(flname) - posn)
//
//    'get filename without extension
//    posn = InStr(fName, ".")
//        If posn <> 0 Then
//            fName = Left(fName, posn - 1)
//        End If
//    GetFileName = fName
//End Function

std::wstring doubleToString(double d)
{
	// Objekt auf die Klasse stringstream erstellen.
	std::wstringstream ss; 
	ss << d;
	return (ss.str());
}