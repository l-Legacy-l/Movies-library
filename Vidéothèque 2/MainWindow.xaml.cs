using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vidéothèque_2
{
    public partial class MainWindow : Window
    {
        Film monFilm;
        ObservableCollection<Film> listeFilm;
        SecondWindow fen2;
        string path; 
        private MessageBoxResult result;

        public MainWindow() // Initialisation
        {
            InitializeComponent();

            listeFilm = new ObservableCollection<Film>(); //Initialisation de la liste qui va contenir les films
            path= System.AppDomain.CurrentDomain.BaseDirectory;// Variable pour enregistrer le chemin courant

            // On place une image par défaut
            ajoutImage(path +"noimage.png");

            //this.DataContext = listeFilm;
            string saveDirectory = path + "Mes sauvegardes";
            try
            {
                if (Directory.GetFileSystemEntries(saveDirectory).Length != 0) //Si une sauvegarde existe déjà
                {
                    result = MessageBox.Show("Voulez-vous charger la sauvegarde de votre vidéothèque ? ", "Charger la sauvegarde ?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes) // Si on clique sur Oui
                    {
                        load(null, null); // On charge la sauvegarde 
                    }
                }
            }
            catch(DirectoryNotFoundException e)
            {
                MessageBox.Show("Le repertoire de sauvegarde a été supprimé, celle-ci ne fonctionnera donc pas", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ParcourirListener(object sender, RoutedEventArgs e) // Evenement si on clique sur le bouton parcourir pour sélectionner une image
        {
            OpenFileDialog ofd = new OpenFileDialog(); // La fenêtre parcourir
            ofd.Title = "Sélectionner une image";
            ofd.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif| All Files| *.*";
            ofd.RestoreDirectory = true; 

            var resultat = ofd.ShowDialog(); // variable qui va permettre de savoir si on a cliqué sur OK (true)
            if (resultat == true)
            {
                ajoutImage(ofd.FileName);
            }
        }

        //Méthode pour ajouter une image
        private void ajoutImage(string path)
        {
            try
            {
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(path); // Chemin relatif
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();

                image.Source = src;
                image.Stretch = Stretch.Uniform;
            }
            catch(FileNotFoundException e)
            {
                MessageBox.Show("Impossible de trouver l'image par défaut celle-ci a peut-être été supprimé","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void ajouterFilmListener(object sender, RoutedEventArgs e) // Evenement lancé lorsqu'on ajoute un film
        {
            if (textTitre.Text != "")
            {
                result = MessageBoxResult.Yes; // Par défaut on peut ajouter un film

                //On avertir l'utilisateur s'il manque un champ
                if (textAuteur.Text == "" || date.Text == "" || textGenre.Text == "" || textResume.Text == "")
                {
                    result = MessageBox.Show("Vous êtes sur le point d'ajouter un film avec au moins un champ manquant, voulez-vous tout de même ajouter ce film ?", "Confirmation d'un ajout", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                }
                if (result == MessageBoxResult.Yes) // Si on a cliqué sur oui
                {
                    //On crée un film
                    monFilm = new Film(textTitre.Text, textAuteur.Text, textGenre.Text, date.Text, checkIsVu.IsChecked, textResume.Text, image.Source.ToString());
                    //On ajoute le film crée dans la liste de film
                    listeFilm.Add(monFilm);
                    //Confirmation d'ajout
                    MessageBox.Show("Le film " + textTitre.Text + " a bien été ajouté !", "Confirmation d'ajout", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                //On gère le mode si on veut nettoyer les champs après chaque ajout
                if (clearMode.IsChecked == true)
                {
                    textTitre.Text = "";
                    textAuteur.Text = "";
                    date.Text = "";
                    textResume.Text = "";
                    textGenre.Text = "";
                    checkIsVu.IsChecked = false;

                    ajoutImage(path + "noimage.png");
                }

            }

            else
            {
                MessageBox.Show("Impossible d'ajouter un film sans titre", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void listenerVideotheque(object sender, RoutedEventArgs e) // Si on veut consulter la vidéothèque
        {
            fen2 = new SecondWindow(listeFilm, this); // On instancie la fenêtre contenant la datagrid  avec la liste de film courante
            fen2.Show();  //On affiche l'autre

        }

        public void save(object sender, RoutedEventArgs e) //Méthode pour sauvegarder la vidéothèque
        {
            try
            {
                //Gestion de plusieurs vidéothèques
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = path + "Mes sauvegardes"; //Dossier contenant les saves par défault
                sfd.Title = "Sélectionner un repertoire pour la sauvegarde";
                sfd.Filter = "Video Library(*.vl) | *.vl | All Files | *.* ";
                sfd.RestoreDirectory = true;
                var resultat = sfd.ShowDialog();

                string fullFilePath = path + "Mes sauvegardes"; //On stocke le chemin où les sauvegardes sont contenues
                if (resultat ==true)
                {
                    string pathOnly = System.IO.Path.GetDirectoryName(sfd.FileName); //On stocke le nom du repertoire choisis

                    if(fullFilePath == pathOnly) //Si le nom du repertoire est le même que celui voulu on peut save
                    {
                        //On ouvre un flux, save.vid sera le fichier de sauvegarde qui sera écrasé à chaque nouvelle sauvegarde
                        FileStream flux = new FileStream(sfd.FileName, FileMode.Create);

                        BinaryFormatter bf = new BinaryFormatter(); //Instance de classe BinaryFormater pour enregistrer ma liste

                        bf.Serialize(flux, listeFilm); //On sérialize la liste (on sauve)
                        flux.Close(); //Je ferme le flux (IMPORTANT)

                        MessageBox.Show("Votre vidéothèque a bien été sauvegardé !", "Sauvegarde", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Vous devez être dans le repertoire /Mes sauvegardes/  pour pouvoir enregistrer vos vidéothèques","Impossible de sauvegarder",MessageBoxButton.OK,MessageBoxImage.Warning);
                    }    
                }
            }
            catch (IOException ex) // Si il y a une erreur d'entrée/sortie
            {
                MessageBox.Show(ex.Message); // on affiche l'erreur
            }
        }

        private void load(object sender, RoutedEventArgs e) // Méthode pour charger la sauvegarde
        {
            result = MessageBox.Show("Attention les données en cours non sauvegardées seront perdus, voulez vous poursuivre le chargement ? ", "Charger une sauvegarde", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ObservableCollection<Film> tempListe = new ObservableCollection<Film>(); //Je créé une liste temporaire qui va contenir les films sauvé
                try
                {
                    OpenFileDialog ofd = new OpenFileDialog(); //On instancie cette classe pour ouvrir un FileDialog
                    ofd.InitialDirectory = path + "Mes sauvegardes"; //Dossier contenant les saves par défault
                    ofd.Filter = "Video Library (*.vl)| *.vl| All Files| *.*";
                    ofd.Title = "Selectionner un fichier de sauvegarde";
                    ofd.RestoreDirectory = true;

                    var resultat = ofd.ShowDialog();

                    if(resultat == true)
                    {
                        listeFilm.Clear(); // On nettoye la liste autrement on va charge la sauvegarde et la liste de film

                        FileStream flux = new FileStream(ofd.FileName, FileMode.Open); // On crée un flux et on ouvre le fichier de save
                        BinaryFormatter bf = new BinaryFormatter();
                        try
                        {
                            //Je récupère la liste de mon fichier en désérialisant le fichier et en castant les données en observablecollection et je stocke ça dans une liste de film temporaire
                            tempListe = (ObservableCollection<Film>)bf.Deserialize(flux);

                        }
                        catch (InvalidCastException inv) // Si on a pas réussi a caster en ObservableCollection
                        {
                            MessageBox.Show(inv.Message + "\nErreur lors du chargement de la sauvegarde");
                        }

                        foreach (Film film in tempListe) // Pour chaque film dans ma liste temporaire
                        {
                            //On réinsctancie les films 
                            Film newFilm = new Film(film.Titre, film.Auteur, film.Genre, film.Date, film.IsVu, film.Resume, film.Chemin); //Je réinstancie mes films avec les informations de la liste, sinon il considère que je n'ai aucune instances
                            listeFilm.Add(newFilm); //j'ajoute mon instance dans ma liste
                        }

                        flux.Close(); //je ferme mon fichier
                    }
                    
                }
                catch (FileNotFoundException ex) // Si on a pas trouvé le fichier de sauvegarde
                {
                    MessageBox.Show(ex.Message + "\nErreur lors du chargement de la sauvegarde");
                }
                
            }
        }

        protected override void OnClosing(CancelEventArgs e) // Méthode pour gérer l'extinction du programme
        {
            if(listeFilm.Count != 0 ) // Si on a pas remplit de film, inutile de demander à sauvegarder
            {
                result = MessageBox.Show("Attention, toutes données non sauvegardées seront perdu, voulez vous sauvegarder avant de quitter ?", "Attention", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    save(null, null);
                    base.OnClosing(e);
                }
                else if (result == MessageBoxResult.No)
                {
                    base.OnClosing(e);
                }
                else //On annule la fermeture
                {
                    e.Cancel = true;
                }
                if (fen2 != null) //Si on quitte mais qu'on a toujours la fenêtre de la datagrid ouverte, on gère également sa fermeture
                {
                    fen2.Close();
                }
            }
        }

        //Si on clique sur l'option pour nettoyer les champs
        private void clearAll_Click(object sender, RoutedEventArgs e) 
        {
            textTitre.Text = "";
            textAuteur.Text = "";
            date.Text = "";
            textResume.Text = "";
            textGenre.Text = "";
            checkIsVu.IsChecked = false;

            ajoutImage(path+"noimage.png");            

        }

        public ObservableCollection<Film> ListeFilm
        {
            get
            {
                return listeFilm;
            }

            set
            {
                listeFilm = value;
            }
        }

        private void googleSearch_Click(object sender, RoutedEventArgs e)
        {
            if(textTitre.Text != "")
            {
                string url = "http://www.google.be/search?q=" + textTitre.Text + " poster"+ "&source=lnms&tbm=isch";  
                System.Diagnostics.Process.Start(url);
            }
            else
            {
                MessageBox.Show("Veuillez renseigner le titre du film pour effectuer la recherche", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AllocineSearch_Click(object sender, RoutedEventArgs e)
        {
            if (textTitre.Text != "")
            {
                string url = "http://www.allocine.fr/recherche/1/?q=" + textTitre.Text;
                System.Diagnostics.Process.Start(url);
            }
            else
            {
                MessageBox.Show("Veuillez renseigner le titre du film pour effectuer la recherche", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ce logicel a été developpé par Fabio Cumbo, étudiant à la HEH.\nVous pouvez me contacter à l'adresse: fabio.cumbo@std.heh.be", "À propos", MessageBoxButton.OK,MessageBoxImage.Question);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ce logiciel est un gestionnaire de collection de films, il va vous permettre d'ajouter tous les films que vous souhaitez simplement en remplissant chaque champs caractérisant votre film.\n\nVous pouvez ensuite consulter à tout moment votre collection de film sur le bouton Consulter votre vidéothèque. Vous pouvez également enregistrer votre liste de films en cours et la charger.\n\nVous pouvez chercher le synopsis du film directement sur Allocine ou encore chercher l'affiche du film voulu sur Google image en sélectionnant Options dans la barre d'outils. Vous avez la possibilité de supprimer et éditer un film directement sur le tableau de la liste de vos films.","Utilisation du logiciel",MessageBoxButton.OK,MessageBoxImage.Question );
        }
    }


}
  

