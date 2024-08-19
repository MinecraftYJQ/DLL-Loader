#include <windows.h>
#include <tlhelp32.h>
#include <tchar.h>
#include <stdio.h>

// 获取目标进程ID
DWORD GetTargetProcessID(const TCHAR* targetProcessName)
{
    DWORD processID = 0;
    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (hSnapshot != INVALID_HANDLE_VALUE)
    {
        PROCESSENTRY32 processEntry;
        processEntry.dwSize = sizeof(PROCESSENTRY32);
        if (Process32First(hSnapshot, &processEntry))
        {
            do
            {
                if (_tcsicmp(processEntry.szExeFile, targetProcessName) == 0)
                {
                    processID = processEntry.th32ProcessID;
                    break;
                }
            } while (Process32Next(hSnapshot, &processEntry));
        }
        CloseHandle(hSnapshot);
    }
    return processID;
}

// 远程线程注入DLL
BOOL InjectDll(DWORD processID, const TCHAR* dllPath)
{
    HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, processID);
    if (hProcess == NULL)
    {
        return FALSE;
    }

    LPVOID dllPathAddress = VirtualAllocEx(hProcess, NULL, _tcslen(dllPath) * sizeof(TCHAR), MEM_COMMIT, PAGE_READWRITE);
    if (dllPathAddress == NULL)
    {
        CloseHandle(hProcess);
        return FALSE;
    }

    SIZE_T bytesWritten;
    if (!WriteProcessMemory(hProcess, dllPathAddress, dllPath, _tcslen(dllPath) * sizeof(TCHAR), &bytesWritten))
    {
        VirtualFreeEx(hProcess, dllPathAddress, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return FALSE;
    }

    HMODULE kernel32Module = GetModuleHandle(_T("kernel32.dll"));
    LPTHREAD_START_ROUTINE loadLibraryFunction = (LPTHREAD_START_ROUTINE)GetProcAddress(kernel32Module, "LoadLibraryW");
    HANDLE hRemoteThread = CreateRemoteThread(hProcess, NULL, 0, loadLibraryFunction, dllPathAddress, 0, NULL);
    if (hRemoteThread == NULL)
    {
        VirtualFreeEx(hProcess, dllPathAddress, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return FALSE;
    }
    return TRUE;
}

int main(int argc, char* argv[])
{
    if (argc > 2) {
        wchar_t targetProcessName[_MAX_PATH];
        wchar_t dllPath[_MAX_PATH];

        mbstowcs_s(nullptr, targetProcessName, argv[1], _MAX_PATH);
        mbstowcs_s(nullptr, dllPath, argv[2], _MAX_PATH); 

        DWORD targetProcessID = GetTargetProcessID(targetProcessName);
        if (targetProcessID != 0)
        {
            if (InjectDll(targetProcessID, dllPath))
            {
                printf("DLL injected successfully.");
            }
            else
            {
                printf("Failed to inject DLL.");
            }
        }
        else
        {
            printf("Target process not found.");
        }
    }
    else {
        MessageBox(NULL, L"数据异常!", L"错误", 0 + 16);
    }
    return 0;
}