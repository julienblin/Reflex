// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValuesSeeder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public class DomainValuesSeeder : BaseSeeder
    {
        // ReSharper disable CoVariantArrayConversion
        private readonly IDictionary<DomainValueCategory, object[]> _values = new Dictionary<DomainValueCategory, object[]>
        {
            {
                DomainValueCategory.ApplicationType,
                new object[]
                    {
                        new object[] { "Application maison", ColorPalette.Green },
                        new object[] { "COTS", ColorPalette.LightBlue },
                        new object[] { "Package", ColorPalette.DarkBlue },
                        new object[] { "Saas", ColorPalette.DarkViolet },
                        new object[] { "SAP (module)", ColorPalette.Violet }
                    }
            },
            {
                DomainValueCategory.ApplicationStatus,
                new object[]
                    {
                        new object[] { "Potentiel", ColorPalette.Yellow },
                        new object[] { "En construction", ColorPalette.LightGreen },
                        new object[] { "En production", ColorPalette.StrongGreen },
                        new object[] { "Retiré", ColorPalette.Orange }
                    }
            },
            {
                DomainValueCategory.ApplicationCriticity,
                new object[]
                    {
                        new object[] { "1", ColorPalette.Red },
                        new object[] { "2", ColorPalette.DarkOrange },
                        new object[] { "3", ColorPalette.Orange }
                    }
            },
            {
                DomainValueCategory.ApplicationCategory,
                new object[]
                    {
                        new object[] { "4GL", Color.Empty, "Base de données orienté pour les utilisateurs permettant de créer facilement des applications et rapports" },
                        new object[] { "Application Maison", Color.Empty, "Applications associées à l'exhibit 16" },
                        new object[] { "BI", Color.Empty, "Logiciels spécialisé pour l'analyse de données" },
                        new object[] { "Browser", Color.Empty, "Logiciel de type Internet Explorer" },
                        new object[] { "Budget", Color.Empty, "Gestion de budget personnels" },
                        new object[] { "Bureautique", Color.Empty, "Logiciel d'édition tel que la suite Office pour les employés de bureaux" },
                        new object[] { "CAD", Color.Empty, "Logiciel de conception assisté par ordinateur tel que Autocad" },
                        new object[] { "CODEC", Color.Empty, "Composantes permettant d'interpréter des fichiers audio et video" },
                        new object[] { "Communication", Color.Empty, "Logiciel permettant de communiquer via Internet tel que MSN Messenger et Skype" },
                        new object[] { "Component", Color.Empty, "Composantes internes d'applications ou de Windows tel que le Windows .Net Framework" },
                        new object[] { "Comptabilité", Color.Empty, "Logiciels de compabilité" },
                        new object[] { "Document Management", Color.Empty, "Gestion documentaire accé sur la gestion et le cataloguage des documents" },
                        new object[] { "Généalogie", Color.Empty, "Logiciels spécialisés pour gérer des arbres généalogique" },
                        new object[] { "Hardware", Color.Empty, "Logiciels associés aux matériels des ordinateurs et portable tel que la captation d'image de caméra, scanners et imprimantes" },
                        new object[] { "Help Desk", Color.Empty, "Logiciels assurant des services de contrôle à distance des postes de travails pour les Help Desk" },
                        new object[] { "HR", Color.Empty, "Gestion des ressources humaines" },
                        new object[] { "ID Card", Color.Empty, "Production de cartes d'identité" },
                        new object[] { "Image Editing", Color.Empty, "Édition d'image et de fichiers multimédia." },
                        new object[] { "Immeuble", Color.Empty, "Gestion des infrastructure des immeubles tel que l'air climatisé, électricité et gas" },
                        new object[] { "Inconnu", Color.Empty, "Logiciels impossible à identifier basé sur l'information disponible. Une rencontre avec les usagers ou des recherches plus approfondies sont nécessaires." },
                        new object[] { "Jeux", Color.Empty, string.Empty },
                        new object[] { "Labeling", Color.Empty, "Production d'étiquettes" },
                        new object[] { "Langues", Color.Empty, "Logiciel de traduction, d'aide et de formation linguistique" },
                        new object[] { "Logistic", Color.Empty, "Gestion des matériels danjeureux, gestion des l'équipement, courrier et expédition" },
                        new object[] { "MAP", Color.Empty, "Logiciels de géolocalisation et de cartes" },
                        new object[] { "Multimedia: Audio", Color.Empty, "Edition, création et modification de fichiers  audio" },
                        new object[] { "Multimedia: Collection", Color.Empty, "Gestion de collection de contenu multimédia tel que fichiers audio et vidéo" },
                        new object[] { "Multimedia: Video", Color.Empty, "Edition, création et modification de fichiers  vidéo" },
                        new object[] { "Multimedia: Viewer", Color.Empty, "Logiciels spécialisés pour visualiser des fichiers multimédia." },
                        new object[] { "Office Addin", Color.Empty, "Petits logiciels s'intégrant à la suite Office." },
                        new object[] { "One Drop", Color.Empty, "Logiciels spécialisés pour One Drop et la gestion des dons de charités" },
                        new object[] { "PDF Tool", Color.Empty, "Logiciels spécialisés pour éditer et/ou visualiser des fichiers Adobe Acrobat PDF" },
                        new object[] { "Physical conditionning", Color.Empty, "Gestion des patients et traitements physiologique" },
                        new object[] { "Production: Autre", Color.Empty, "Logiciels associé à des équipements de production spécialisés tel que simulation d'automates et de composantes automatisées" },
                        new object[] { "Production: Color tool", Color.Empty, "Utilitaires permettant de gérer les standards de couleurs selon les différents médiums ou support tel que les costumes, imprimantes, écrans et vidéos" },
                        new object[] { "Production: Costume", Color.Empty, "Support à la création des costumes etl que visualisation de manequins virtuelle et création de dentelle." },
                        new object[] { "Production: Lighting", Color.Empty, "Gestion et conception des éclairages" },
                        new object[] { "Production: Music", Color.Empty, "Logiciels spécialisés pour la composition musicale, interface avec des instruments et consoles de son." },
                        new object[] { "SAP", Color.Empty, "Logiciels associés à SAP" },
                        new object[] { "Security", Color.Empty, "Gestion des mots de passes et sécurité des fichiers." },
                        new object[] { "Service alimentaire", Color.Empty, "Calcul de al valeur nutritive des aliments et aides à la conception des menus." },
                        new object[] { "Taxation - Paye", Color.Empty, "Logiciel de préparation des rapports d'impôts canadiens et logiciel de gestion des taxes." },
                        new object[] { "Tool", Color.Empty, "Catégories générales associé à des outils spécialisés Windows te que Macro de claviers, amélioration de l'interface Windows, transfert FTP, etc…" },
                        new object[] { "Tool File", Color.Empty, "Logiciel spécialisés pour copier, transféres et compresser des fichiers" },
                        new object[] { "Transportation", Color.Empty, "Gestion de la capacité de l'espace de stokcage des camions et transport routier et maritime" },
                        new object[] { "Wallpaper", Color.Empty, "Logiciels spécialisés de Wall paper corporatifs" },
                        new object[] { "Web Design", Color.Empty, "Production et développement de contenu de site Internet. Ceci inclus l'édition HTML, Web et le développement de composantes." }
                    }
            },
            {
                DomainValueCategory.ApplicationUserRange,
                new[] { "1", "2-5", "6-10", "11-99", "100-999", "1000+" }
            },
            {
                DomainValueCategory.ApplicationCertification,
                new[] { "Certifié", "Non-certifié" }
            },
            {
                DomainValueCategory.IntegrationNature,
                new[] { "Synchrone", "Asynchrone" }
            },
            {
                DomainValueCategory.ContactType,
                new[] { "Directeur TI", "Analyste d'affaire", "Architecte" }
            },
            {
                DomainValueCategory.EventType,
                new[] { "Divers 1", "Divers 2" }
            },
            {
                DomainValueCategory.UserCompany,
                new[] { "Compagnie 1", "Compagnie 2" }
            },
            {
                DomainValueCategory.TechnologyType,
                new[] { "Language", "Base de données", "Système d'exploitation", "Navigateur" }
            },
            {
                DomainValueCategory.Environment,
                new[] { "Dev", "Test", "Preprod", "Prod" }
            },
            {
                DomainValueCategory.ServerRole,
                new[] { "Serveur d'application", "Serveur web", "Serveur de BD" }
            },
            {
                DomainValueCategory.ServerStatus,
                new object[]
                    {
                        new object[] { "En service", ColorPalette.Green },
                        new object[] { "Hors service", ColorPalette.DarkOrange }
                    }
            },
            {
                DomainValueCategory.ServerType,
                new[] { "Physique", "Virtuel" }
            },
            {
                DomainValueCategory.ContactRoleInApp,
                new[] { "Contact Principal", "Approbateur" }
            }
        };
        //// ReSharper restore CoVariantArrayConversion

        public override int Priority { get { return 0; } }

        protected override void SeedImpl()
        {
            foreach (var category in _values.Keys)
                for (var i = 0; i < _values[category].Count(); i++)
                {
                    var value = _values[category][i];
                    if (value is string)
                        Session.Save(new DomainValue { Category = category, Name = (string)value, DisplayOrder = i });
                    else
                    {
                        var fullValue = (object[])_values[category][i];
                        var dv = new DomainValue { Category = category, Name = (string)fullValue[0], DisplayOrder = i };
                        if (fullValue.Length > 1)
                            dv.Color = (Color)fullValue[1];
                        if (fullValue.Length > 2)
                            dv.Description = (string)fullValue[2];

                        Session.Save(dv);
                    }
                }
        }
    }
}