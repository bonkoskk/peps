#ifndef DLL_DEFINE
#define DLL_DEFINE

#ifdef __GNUC__
	#define DLLEXP __attribute__((visibility("default")))
#else
	#define DLLEXP __declspec(dllexport)
#endif

#endif // DLL_DEFINE