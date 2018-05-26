using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Vidéothèque_2
{
    [Serializable]
    public class Film 
    {
        //Attribut de la classe
        private string titre;
        private string auteur;
        private string resume;
        private string genre;
        private string date;
        private string chemin;
        private bool? isVu;


        //Constructeur
        public Film(string titre, string auteur, string genre, string date,bool? isVu,string resume,String chemin)
        {
            this.Titre = titre;
            this.Auteur = auteur;
            this.Resume = resume;
            this.Genre = genre;
            this.Chemin = chemin;
            this.Date = date;
            this.IsVu = isVu;
        }

        public string Titre
        {
            get
            {
                return titre;
            }

            set
            {
                titre = value;
              
            }
        }

        public string Auteur
        {
            get
            {
                return auteur;
            }

            set
            {
                auteur = value;
                
            }
        }

        public string Resume
        {
            get
            {
                return resume;
            }

            set
            {
                resume = value;
               
            }
        }

        public string Genre
        {
            get
            {
                return genre;
            }

            set
            {
                genre = value;
                
            }
        }

        public string Date
        {
            get
            {
                return date;
            }

            set
            {
                date = value;
              
            }
        }

        public string Chemin
        {
            get
            {
                return chemin;
            }

            set
            {
                chemin = value;
               
            }
        }

        public bool? IsVu
        {
            get
            {
                return isVu;
            }

            set
            {
                isVu = value;
            }
        }

        public override string ToString()
        {
            return (Titre +" " + Auteur +" "+ Genre +" " + Date + " "+ Resume +" "+ Chemin);
        }


    }
}
   
