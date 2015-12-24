using System;

namespace StockDemo
{
public class OhlcModel : ViewModelBase
{
    private double open;
    private double high;
    private double low;
    private double close;
    private DateTime date;

    public OhlcModel(params double[] values)
    {
        this.open = values[0];
        this.high = values[1];
        this.low = values[2];
        this.close = values[3];
    }

    public DateTime Date
    {
        get
        {
            return this.date;
        }
        set
        {
            if (this.date == value)
            {
                return;
            }

            this.date = value;
            this.OnPropertyChanged("Date");
        }
    }

    public double Open
    {
        get
        {
            return this.open;
        }
        set
        {
            if (this.open == value)
            {
                return;
            }

            this.open = value;
            this.OnPropertyChanged("Open");
        }
    }

    public double High
    {
        get
        {
            return this.high;
        }
        set
        {
            if (this.high == value)
            {
                return;
            }

            this.high = value;
            this.OnPropertyChanged("High");
        }
    }

    public double Low
    {
        get
        {
            return this.low;
        }
        set
        {
            if (this.low == value)
            {
                return;
            }

            this.low = value;
            this.OnPropertyChanged("Low");
        }
    }

    public double Close
    {
        get
        {
            return this.close;
        }
        set
        {
            if (this.close == value)
            {
                return;
            }

            this.close = value;
            this.OnPropertyChanged("Close");
        }
    }
}
}
