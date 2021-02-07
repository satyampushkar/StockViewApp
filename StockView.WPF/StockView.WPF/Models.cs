using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace StockView.WPF
{
    public class Stock : INotifyPropertyChanged
    {
        private Guid _id;
        private string _name;
        private string _symbol;
        private bool _tracking = false;

        public Stock(string name, string symbol, string id = null)
        {
            if (id != null)
            {
                _id = Guid.Parse(id);
            }
            else
            {
                _id = Guid.NewGuid();
            }
            Name = name;
            Symbol = symbol;

        }
        public Guid Id
        {
            get { return _id; }
            //set
            //{
            //    if (value != _id)
            //    {
            //        _id = value;
            //        NotifyPropertyChanged();
            //    }
            //}
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                if (value != _symbol)
                {
                    _symbol = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Tracking
        {
            get { return _tracking; }
            set
            {
                if (value != _tracking)
                {
                    _tracking = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName ="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class StockDetail : Stock
    {
        private float _currentPrice { get; set; }
        private float _percentageChangeFromLastDay { get; set; }
        private float _high52Week { get; set; }
        private float _low52Week { get; set; }

        public StockDetail(string id, string name, string symbol, float currentPrice, float percentageChangeFromLastDay
            , float high52Week, float low52Week) : base(name, symbol, id)
        {
            CurrentPrice = currentPrice;
            PercentageChangeFromLastDay = percentageChangeFromLastDay;
            High52Week = high52Week;
            Low52Week = low52Week;
        }
        public float CurrentPrice
        {
            get { return _currentPrice; }
            set
            {
                if (value != _currentPrice)
                {
                    _currentPrice = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public float PercentageChangeFromLastDay
        {
            get { return _percentageChangeFromLastDay; }
            set
            {
                if (value != _percentageChangeFromLastDay)
                {
                    _percentageChangeFromLastDay = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public float High52Week
        {
            get { return _high52Week; }
            set
            {
                if (value != _high52Week)
                {
                    _high52Week = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public float Low52Week
        {
            get { return _low52Week; }
            set
            {
                if (value != _low52Week)
                {
                    _low52Week = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }

    public class Company
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public string companyName { get; set; }        
        public string price { get; set; }
        public string changes { get; set; }
        public string currency { get; set; }
        public string range { get; set; }
    }   
}
