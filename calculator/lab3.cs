using System;
using System.Collections.Generic;

//Родительский класс для числа и операции
interface ISymbol{
    string ToString();
    
}

class Number: ISymbol{
    private double _value;
    public double value{ get{return _value;}}
    public Number(double number){
        _value = number;
    }
    override public string ToString(){
        return value.ToString();
    }
}

class Operation: ISymbol{
    private string _value;
    private int _priority;
    
    public string value{ get{ return _value;}}
    public int priority{ get{ return _priority;}}
    
    public Operation(string operation, int priority){
        _value = operation;
        _priority = priority;
    }
    
}

class ArithmeticExpressionSlicer{
    private readonly string ArithmeticExpression; //Арифметическое выражение записанная в строке
    private int pos; //Текущая позиция для получения элементов
    private bool nextUnary; //Флаг для обозначения унарности следующей операции
    private bool prevIsNumber;  //Флаг для обозначения предыдущего элемента
    
    public ArithmeticExpressionSlicer(string input){
        ArithmeticExpression = input;
        pos = 0;
        nextUnary = true;
        prevIsNumber = false;
    }
    
    public bool CheckNext(){
        if(pos < ArithmeticExpression.Length) return true;
        return false;
    }
    //Метод для получения элемента(операции или числа) из строки
    public ISymbol GetSymbol(){
        if(pos < ArithmeticExpression.Length){
            switch(ArithmeticExpression[pos]){
                case '+':
                    prevIsNumber = false;
                    pos++;
                    if(nextUnary) return new Operation("+_", 3);
                    nextUnary = true;
                    return new Operation("+", 1);
                    
                case '-':
                    prevIsNumber = false;
                    pos++;
                    if(nextUnary) return new Operation("-_", 3);
                    nextUnary = true;
                    return new Operation("-", 1);
                    
                case '*':
                    prevIsNumber = false;
                    pos++;
                    return new Operation("*", 2);
                    
                case '/':
                    prevIsNumber = false;
                    pos++;
                    return new Operation("/", 2);
                    
                case '(':
                    prevIsNumber = false;
                    nextUnary = true;
                    pos++;
                    return new Operation("(", 4);
                    
                case ')':
                    prevIsNumber = false;
                    nextUnary = true;
                    pos++;
                    return new Operation(")", 4);
                    
                case ' ':
                    pos++;
                    break;
                    
                default:
                    nextUnary = false;
                    
                    if (!char.IsNumber(ArithmeticExpression[pos]))
                        throw new ArgumentException($"unknown symbol: {ArithmeticExpression[pos]}");

                    if (prevIsNumber)
                    {
                        throw new InvalidOperationException("missing operation!");
                    }
                    prevIsNumber = true;

                    int start_pos = pos;
                    pos++;
                    while (pos < ArithmeticExpression.Length && char.IsDigit(ArithmeticExpression[pos]))
                    {
                        pos++;
                    }

                    string str_number = ArithmeticExpression.Substring(start_pos, pos - start_pos);
                    double number = Convert.ToDouble(str_number);
                    return new Number(number);
            }
        }
        throw new ArgumentException("input error");
    }
    //Перевод строки в польскую нотацию
    public Queue<ISymbol> GetPolishNotation(){
        //При выполнении следующих действий над исходной строкой с
        //арифметическим выражением мы будем использовать вспомогательную
        //структуру данных — стек(для операций) и очередь для записи результата.
        Queue<ISymbol> res = new Queue<ISymbol>();
        Stack<ISymbol> operators_stack = new Stack<ISymbol>();
        while(CheckNext()){
            ISymbol symbol = GetSymbol();
            //1)Если очередной элемент — число или имя переменной, то он сразу переносится в результирующую строку.
            if(symbol.GetType() == typeof(Number)){
                res.Enqueue(symbol);
                continue;
            }
            //2) Открывающаяся скобка всегда помещается в стек
            Operation symbol_op = (Operation) symbol;
            if(symbol_op.value == "("){
                operators_stack.Push(symbol_op);
                continue;
            }
            //3) Если встретилась закрывающаяся скобка, то из стека извлекаются все знаки операций до первой открывающейся скобки и в порядке извлечения переносятся в результирующую строку, а открывающаяся скобка в вершине стека уничтожается.
            if(symbol_op.value == ")"){
                do{
                    try{
                        symbol_op = (Operation) operators_stack.Peek();
                    }catch(InvalidOperationException){
                        throw new InvalidOperationException("there is no opening paranthesis");
                    }
                    
                    if(symbol_op.value == "("){
                        operators_stack.Pop();
                        break;
                    }
                    
                    res.Enqueue(symbol_op);
                    operators_stack.Pop();
                }while(true);
                continue;
            }
            //4) Если встречается знак операции, то
            do{
                //a)если в вершине стека знак операции с более низким приоритетом или открывающаяся скобка или ничего нет, то встретившийся знак заносится в стек;
                Operation top_op;
                try{
                    top_op = (Operation) operators_stack.Peek();
                } catch (InvalidOperationException e){
                    break;
                } 
                
                if (top_op.value == "(")
                {
                    break;
                }
        
                //b)знаки операций из стека, пока приоритет их больше или равен приоритету данного знака, извлекаются из стека и заносятся в результирующую строку, после чего рассматриваемый знак записывается в стек.
                if (symbol_op.priority <= top_op.priority)
                {

                    operators_stack.Pop();
                    res.Enqueue(top_op);
                    continue;
                }
                
                break;
            }while(operators_stack.Count > 0);
            operators_stack.Push(symbol_op);
        }
        
        //5) Если исходная строка исчерпана, то оставшиеся в стеке знаки операций по порядку переносятся в результирующую строку.
        while (operators_stack.Count > 0)
        {
            Operation top_op = (Operation)operators_stack.Pop();
    
            // Если текущий знак это скобка, значит где-то есть незакрытые скобки
            if (top_op.value == ")" || top_op.value == "(")
            {
                throw new InvalidOperationException("parentheses missing");
            }

            res.Enqueue(top_op);
        }
        return res;
    }
}




class Program
{
    //Вычисление арифметического выражения в польской нотации
    static double EvaluateExpression(Queue<ISymbol> expression){
        //В качестве вспомогательной структуры данных используется стек
        Stack<double> numbers = new Stack<double>();
        double right, left;
        while(expression.Count > 0){
            //1) Если встретившийся элемент — операнд, то он помещается в стек.
            ISymbol symbol = expression.Dequeue();
            if(symbol.GetType() == typeof(Number)){
                numbers.Push(((Number)symbol).value);
                continue;
            }
            //2) Если очередной элемент — знак бинарной операции, то из стека извлекаются два верхних элемента и над ними выполняется операция, результат которой заносится в стек,
            if(symbol.GetType() == typeof(Operation)){
                Operation op = (Operation)symbol;
                switch(op.value){
                    case "+":
                        right = numbers.Pop();
                        left = numbers.Pop();
                        numbers.Push(left + right);
                        break;
    
                    case "-":
                        right = numbers.Pop();
                        left = numbers.Pop();
                        numbers.Push(left - right);
                        break;
    
                    case "*":
                        right = numbers.Pop();
                        left = numbers.Pop();
                        numbers.Push(left * right);
                        break;
    
                    case "/":
                        right = numbers.Pop();
                        left = numbers.Pop();
                        if (right == 0)
                        {
                            throw new DivideByZeroException("can't divide by zero");
                        }
                        numbers.Push(left / right);
                        break;
    
                    // унарная операция выполняется над верхним элементом.
                    case "-_":
                        right = numbers.Pop();
                        numbers.Push(-right);
                        break;
    
                    case "+_":
                        break;
                }
            }
        }
        //В конце концов в стеке остается одно число — результат.
        double result = numbers.Pop();
        return result;
    }

    static void Main() {
        Console.Write("> ");
        string input = Console.ReadLine();
        // Создаем класс для выделения токенов в выражении
        ArithmeticExpressionSlicer slicer = new ArithmeticExpressionSlicer(input);

        // Переводим инфиксную нотацию в польскую
        Queue<ISymbol> expr = slicer.GetPolishNotation();

        // Вычисляем выражение
         double result = EvaluateExpression(expr);
        // Выводим результат
        Console.WriteLine(result.ToString());
    }
}