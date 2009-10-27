#pragma once
#include <malloc.h>

#define lslib_xmalloc(len)	debug_malloc( __FILE__,__LINE__, (len))
#define lslib_xrealloc(ptr,len) debug_realloc(__FILE__,__LINE__, (ptr), (len))
#define lslib_xstrdup(str1)     DBstrdup(__FILE__, __LINE__, (str1))