#ifndef Windows_CE_ASSERT
#define Windows_CE_ASSERT
#pragma once
#include <windows.h>

/* clach04 WinCE wierd stuff */

/* Windows CE doesn't have an assert() */
/* There is an ASSERT() that is supposed to be similar */
/* but it does odd things with unicode strings that I don't know about */
/* Add a simple message box to display the assert failure but carry on running */

extern void wince_assert_message(char * mesgTitle, char * mesgText);
extern void wince_assert_message_detail(char * mesgTitle, char * mesgText, char * filename, int linenum);

#define assert(x) { if ( !(x) ) wince_assert_message_detail("Assert Failed!" ,  #x , __FILE__, __LINE__ ); }

/*
Disable assert
#define assert(x) 

Simple popup
#define assert(x) { if ( !(x) ) wince_assert_message("Assert Failed!" ,  #x ); }

popup as above with src file and line # Can't get function name to work :-(
#define assert(x) { if ( !(x) ) wince_assert_message_detail("Assert Failed!" ,  #x , __FILE__, __LINE__ ); }

*/

#endif
