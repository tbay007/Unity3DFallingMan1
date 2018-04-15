using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class HighScoreModel 
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        public string ScoreDate { get; set; }
    }
}
