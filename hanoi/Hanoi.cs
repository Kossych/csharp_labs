using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

//Класс башни, который содержит в себе стек колец (используем стек, тк мы перекладываем только верхнее кольцо за раз)
class Tower{
    private Stack<int> _rings;
    
    public Tower(int rings_amount){
        _rings = new Stack<int>(rings_amount);
        for(int i=rings_amount; i>0; i--){
            _rings.Push(i);
        }
    }
    
    public void TowerPrint(){
        int[] towerArr = _rings.ToArray();
        for (int i = towerArr.Length - 1; i >= 0; i--)
        {
            Console.Write("{0} ", towerArr[i]);
        }
        Console.WriteLine();
    }
    
    public int Pop(){
        return _rings.Pop();
    }
    
    public void Push(int ring){
        //Если перекладываемое кольцо больше верхнего, вылетает исключение
        if(_rings.Count == 0 || _rings.Peek() >= ring) _rings.Push(ring);
        else throw new InvalidOperationException("ring is too big");
    }
    

    
};

class Towers{
    public Tower[] _towers;
    private int _towers_amount = 3;
    
    public Towers(int rings_amount = 10){
        _towers = new Tower[_towers_amount];
        _towers[0] = new Tower(rings_amount);
        for(int i = 1; i < _towers_amount; i++){
            _towers[i] = new Tower(0);
        }
    }
    
    public void MoveRing(Tower from, Tower to){
        int ring;
        try{
            ring = from.Pop();  
        }catch(InvalidOperationException exception){
            throw exception;
        }
        try{ 
            to.Push(ring);
        }catch(InvalidOperationException exception){
            from.Push(ring);
            throw exception;
        }
    }

    public void MoveRings(int rings_amount, Tower from, Tower to, Tower independ){
        if(rings_amount == 0){
            return;
        }
        MoveRings(rings_amount-1, from: from, to: independ, independ: to);
        MoveRing(from, to);
        MoveRings(rings_amount-1, from: independ, to: to, independ: from);
    }
    
    public void TowersPrint(){
        for(int i = 0; i < _towers_amount; i++){
            Console.Write("Tower {0}: ", i+1);
            _towers[i].TowerPrint();
        }
    }
}

class HahoiTowersTask {
    static void Main() {
        Stopwatch stopWatch = new Stopwatch();
        Console.Write("Enter rings amount : ");
        int rings_amount = Convert.ToInt32(Console.ReadLine());
        Towers towers = new Towers(rings_amount);
        towers.TowersPrint();
        stopWatch.Start();
        
        try{
            towers.MoveRings(rings_amount, towers._towers[0], towers._towers[1], towers._towers[2]);
        }catch (Exception e){
             Console.WriteLine("{0}", e.Message);
        }
        
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        towers.TowersPrint();
        Console.WriteLine("RunTime " + elapsedTime);

    }
};
