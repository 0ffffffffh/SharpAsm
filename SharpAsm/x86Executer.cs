using System;
using System.Text;
using System.Runtime.InteropServices;

/*
 * x86 Assembly Code Execution from inside Managed C# Language
 * Author: Oguz Kartal (0xffffffff)
 * Date: 7/21/2011 1:50 AM
 */

namespace SharpAsm
{
    class x86Executer
    {
        private delegate int FuncPtr();
        private delegate int FuncPtr2(int a, int b);

        private const int PAGE_SIZE = 0x1000;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAlloc(IntPtr addr, UIntPtr size, uint allocType, uint prot);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualFree(IntPtr addr, UIntPtr size, uint freeType);

        private IntPtr codePage;

        private readonly byte[] addx86Code =
        {
            0x8b, 0x44, 0x24,0x04, //MOV EAX, [ESP+4]
            0x03, 0x44, 0x24,0x08, //ADD EAX, [ESP+8]
            0xc3                   //RET
        };

        private readonly byte[] eflagx86Code =
        {
            0x9c, //PUSHFD
            0x58, //POP EAX
            0xc3  //RET
        };

        private bool AllocPage()
        {
            codePage = VirtualAlloc(IntPtr.Zero, new UIntPtr(PAGE_SIZE), 0x1000, 0x40);

            return codePage != IntPtr.Zero;
        }

        private void FreePage()
        {
            VirtualFree(codePage,UIntPtr.Zero,0x8000);
            codePage = IntPtr.Zero;
        }

        private Delegate LoadCodeToMem(byte[] code,Type funcType)
        {
            Marshal.Copy(code, 0, codePage, code.Length);
            return Marshal.GetDelegateForFunctionPointer(codePage, funcType);
        }

        public int x86Add(int first, int second)
        {
            int result = 0;
            FuncPtr2 caller;

            if (AllocPage())
            {
                caller = (FuncPtr2)LoadCodeToMem(addx86Code, typeof(FuncPtr2));
                result = caller(first, second);
                FreePage();
            }
            else
                throw new Exception("Can not allocated page mem");

            return result;

        }

        public int GetEFlags()
        {
            int result = 0;
            FuncPtr caller;

            if (AllocPage())
            {
                caller = (FuncPtr)LoadCodeToMem(eflagx86Code, typeof(FuncPtr));
                result = caller();
                FreePage();
            }
            else
                throw new Exception("Can not allocated page mem");

            return result;
        }



    }
}
