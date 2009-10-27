#pragma once
#include "assert.h"
#include <stdio.h>
#include <windows.h>

/* clach04 WinCE wierd stuff */

/* Windows CE doesn't have an assert() */

/* Simple replacement for MultiByteToWideChar() that avoid length issues */
/* But only supports one code page basic ANSI */
/* Now using mbstowcs() so this can probably be removed */
static void wince_assert_SimpleCharToWide(const char* Src, int Len, wchar_t* Target)
{
   int i;
   for(i = 0;i < Len;++i)
   {
        Target[i] = Src[i];
        Target[i+1] = 0;
   }
}

void wince_assert_message(char * mesgTitle, char * mesgText)
{
	wchar_t msgtitle[100];
	wchar_t msgtext[100];

	wince_assert_SimpleCharToWide(mesgText, strlen(mesgText), msgtext);
	wince_assert_SimpleCharToWide(mesgTitle, strlen(mesgTitle), msgtitle);

	MessageBoxW(NULL, msgtext, msgtitle, MB_OK | MB_ICONERROR | MB_TOPMOST );  /* explictly call Wide version */
}

void wince_assert_message_detail(char * mesgTitle, char * mesgText, char * filename, int linenum)
{
    /* Maybe we chould allocate this space dynmaically? */
    /* For now use large'ish constants */
    wchar_t msgtitle_w[100];
    wchar_t msgtext_w[2048];
    char mesgbuff[2048];
	int i;

    sprintf(mesgbuff, "Assertion: %s\nSrc File: %s\nLine: %d\n", mesgText, filename, linenum);

    i = mbstowcs(msgtext_w, mesgbuff, strlen(mesgbuff) );
	i = mbstowcs(msgtitle_w, mesgTitle, strlen(mesgTitle));

    MessageBoxW(NULL, msgtext_w, msgtitle_w, MB_OK | MB_ICONERROR | MB_TOPMOST );  /* explictly call Wide version */
}

