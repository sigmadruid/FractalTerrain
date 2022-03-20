using System;

public struct Pos
{
    public int Col;
    public int Row;

    public Pos(int col, int row)
    {
        Col = col;
        Row = row;
    }

    public static Pos operator+(Pos a, Pos b)
    {
        return new Pos(a.Col + b.Col, a.Row + b.Row);
    }

    public static Pos operator-(Pos a, Pos b)
    {
        return new Pos(a.Col - b.Col, a.Row - b.Row);
    }

    public static Pos operator*(Pos p, int i)
    {
        return new Pos(p.Col * i, p.Row * i);
    }

    public static Pos operator/(Pos p, int i)
    {
        return new Pos(p.Col / i, p.Row / i);
    }
    
    public override string ToString()
    {
        return string.Format("({0},{1})", Col, Row);
    }
}

