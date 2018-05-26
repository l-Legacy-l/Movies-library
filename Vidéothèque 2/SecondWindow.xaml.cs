using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vidéothèque_2
{

    /// <summary>
    /// Logique d'interaction pour SecondWindow.xaml
    /// </summary>
   
    public partial class SecondWindow : Window
    {

        MainWindow main;
        MessageBoxResult result;
        public SecondWindow(ObservableCollection<Film> list, MainWindow main)
        {
            InitializeComponent();
            DataContext = list; //L'itemsource est bindé
            this.main = main;
        }

        private void listenerBack(object sender, RoutedEventArgs e) //Bouton précédent
        {
            main.Show();
            this.Close();
        }

        private void clearVid_Click(object sender, RoutedEventArgs e)
        {
            
            result = MessageBox.Show("Attention cette opération va effacer votre liste de film, voulez-vous poursuivre ? ", "Confirmation du nettoyage", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                main.ListeFilm.Clear(); //On nettoye la liste des films 
            }
            
        }
    }
}
