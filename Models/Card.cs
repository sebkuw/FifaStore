using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FifaStore.Models
{
    public class Card
    {
        public int ID { get; set; }
        public int FootballerID { get; set; }
        public int CardTypeID { get; set; }
        //public List<int> AvaragePrices = new List<int>();
        private int avaragePriceCounter = 0;
        public int AvaragePriceCounter
        {
            get { return avaragePriceCounter; }
            set { avaragePriceCounter = value; }
        }
        private int fullPrice = 0;

        public int FullPrice
        {
            get { return fullPrice; }
            set { fullPrice = value; }
        }
        private float avaragePrice = 0;
        public float AvaragePrice
        {
            get {
                if(avaragePriceCounter != 0)
                {
                    return fullPrice / avaragePriceCounter;
                }
                else
                {
                    return 0;
                }
            }
            set { avaragePrice = value; }
        }
        private int rateCounter = 0;
        public int RateCounter
        {
            get { return rateCounter; }
            set { rateCounter = value; }
        }
        private int fullRate = 0;
        public int FullRate
        {
            get { return fullRate; }
            set { fullRate = value; }
        }
        private float avarageRate = 0;
        public float AvarageRate
        {
            get
            {
                if (rateCounter != 0)
                {
                    return fullRate / rateCounter;
                }
                else
                {
                    return 0;
                }
            }
            set { avarageRate = value; }
        }
        [Required]
        public Position Position { get; set; }
        [Required, Range(1, 99)]
        public int Overall { get; set; }
        [Required, Range(1, 99)]
        public int Pace { get; set; }
        [Required, Range(1, 99)]
        public int Shooting { get; set; }
        [Required, Range(1, 99)]
        public int Passing { get; set; }
        [Required, Range(1, 99)]
        public int Dribbling { get; set; }
        [Required, Range(1, 99)]
        public int Defending { get; set; }
        [Required, Range(1, 99)]
        public int Physicality { get; set; }
        public virtual Footballer Footballer { get; set; }
        public virtual CardType CardType { get; set; }
        public virtual ICollection<Opinion> Opinions { get; set; }
        public virtual ICollection<Profile> Likers { get; set; }
        public virtual ICollection<Profile> Owners { get; set; }
    }

    public enum Position
    {
        BR,
        CPS, PO, ŚO, LO, CLS,
        PP, PS, PN, LP, LS, LN,
        ŚPD, ŚP, ŚPO, ŚN, N
    }
}