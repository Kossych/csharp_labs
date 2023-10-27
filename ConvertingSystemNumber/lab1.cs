using System;
using System.Linq;

class lab {
    
    static int GetNumberValue(char a){//Функция получения числа по его коду
        if(a >= 48 && a <= 57) return (a-48); //0-9
        if(a >= 65 && a <= 90) return (a-29); // 36 - 61 ABCDEF...  
        if(a >= 97 && a <= 122) return (a-87);// 10 - 35 abcdef...
        return -1;
    }
    
    static char SetNumberValue(ulong a){//Функция получения кода числа 
        if(a >= 0 && a <= 9) return (char)(a+48); //0-9
        if(a >= 10 && a <= 35) return (char)(a+87);// 10 - 35 abcdef...
        if(a >= 36 && a <= 61) return (char)(a+29); // 36 - 61 ABCDEF...  
        return '\n';
    }
    
    static ulong ConvertToDec(string Str, int N){//Функция преобразования числа в десятичную систему
        ulong res = 0;
        int dig = 0;
        for(var i = Str.Length - 1; i >= 0; i--){
            dig = GetNumberValue(Str[i]);
            if(dig > N){//Проверка на корректность разряда числа
                Console.WriteLine("incorrect number digit");
                return 0;
            }
            res += (ulong)dig * (ulong)(Math.Pow(N, Str.Length - i - 1));
        }
        return res;
    }
    
    static string ConvertFromDec(ulong num, int M){//Функция преобразования числа из десятичной системы в произвольную
        string res = "";
        while(num != 0){
            res += SetNumberValue(num % (ulong)M);
            num /= (ulong)M;
        }
        return res;
    }
    
    static string ConvertingSystemNumber(string num, int N, int M) // Функция преобразования числа из системы счисления N в систему счисления M (до 61)
        {
            if(N <= 0 || M <= 0 || N > 61 || M > 61){
                Console.WriteLine("incorrect number digit");
                return num;
            }
            if (N == M) return num;
            num = new string(ConvertFromDec(ConvertToDec(num, N), M).ToCharArray().Reverse().ToArray());
            // Происходит переворачивание числа
            return num;
        }


    static void Main() {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Enter the number system of the number:");
        int N = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Enter the number:");
        String str = Console.ReadLine();
        Console.WriteLine("Enter the desired number system:");
        int M = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine(ConvertingSystemNumber(str, N, M));
    }
}