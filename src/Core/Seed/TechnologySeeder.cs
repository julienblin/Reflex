// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologySeeder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public class TechnologySeeder : BaseSeeder
    {
        private readonly IList<TechnoSeed> _seeds = new List<TechnoSeed>
        {
            new TechnoSeed("Microsoft")
            {
                Children = new[]
                {
                    new TechnoSeed("Visual Basic")
                    {
                        Children = new[]
                        {
                            new TechnoSeed("4", "Language", 2003),
                            new TechnoSeed("5", "Language", 2005),
                            new TechnoSeed("6", "Language", 2008)
                        }
                    },

                    new TechnoSeed(".Net")
                    {
                        Children = new[]
                        {
                            new TechnoSeed("1.1")
                            {
                                Children = new[]
                                {
                                    new TechnoSeed("VB.Net", "Language", 2012),
                                    new TechnoSeed("C#", "Language", 2012)
                                }
                            },

                            new TechnoSeed("2.0")
                            {
                                Children = new[]
                                {
                                    new TechnoSeed("VB.Net", "Language", 2015),
                                    new TechnoSeed("C#", "Language", 2015)
                                }
                            },

                            new TechnoSeed("3.5")
                            {
                                Children = new[]
                                {
                                    new TechnoSeed("VB.Net", "Language", 2018),
                                    new TechnoSeed("C#", "Language", 2018)
                                }
                            },

                            new TechnoSeed("4.0")
                            {
                                Children = new[]
                                {
                                    new TechnoSeed("VB.Net", "Language", 2020),
                                    new TechnoSeed("C#", "Language", 2020)
                                }
                            }
                        }
                    },

                    new TechnoSeed("Internet Explorer")
                    {
                        Children = new[]
                        {
                            new TechnoSeed("5.5", "Navigateur", 2009),
                            new TechnoSeed("6.0", "Navigateur", 2013),
                            new TechnoSeed("7.0", "Navigateur", 2018),
                            new TechnoSeed("8.0", "Navigateur"),
                            new TechnoSeed("9.0", "Navigateur")
                        }
                    },

                    new TechnoSeed("Sql Server")
                    {
                        Children = new[]
                        {
                            new TechnoSeed("2008")
                            {
                                Children = new[]
                                {
                                    new TechnoSeed("R1", "Base de données"),
                                    new TechnoSeed("R2", "Base de données")
                                }
                            },
                            new TechnoSeed("2005", "Base de données"),
                            new TechnoSeed("2000", "Base de données")
                        }
                    },

                    new TechnoSeed("Windows Server")
                    {
                        Children = new[]
                        {
                            new TechnoSeed("2008")
                            {
                                Children = new[]
                                {
                                    new TechnoSeed("R1", "Système d'exploitation"),
                                    new TechnoSeed("R2", "Système d'exploitation")
                                }
                            },
                            new TechnoSeed("2003", "Système d'exploitation"),
                            new TechnoSeed("2000", "Système d'exploitation")
                        }
                    }
                }
            },
            new TechnoSeed("Oracle")
            {
                Children = new[]
                {
                    new TechnoSeed("8i", "Base de données"),
                    new TechnoSeed("9g", "Base de données")
                }
            },
            new TechnoSeed("Open Source")
            {
                Children = new[]
                {
                    new TechnoSeed("Firefox", "Navigateur"),
                    new TechnoSeed("Chrome", "Navigateur")
                }
            }
        };

        public override int Priority { get { return 5; } }

        protected override void SeedImpl()
        {
            foreach (var technoSeed in _seeds)
            {
                technoSeed.Save(Session);
            }
        }

        private class TechnoSeed
        {
            public TechnoSeed(string name)
            {
                Name = name;
                Children = new TechnoSeed[0];
            }

            public TechnoSeed(string name, string type)
            {
                Name = name;
                Type = type;
                Children = new TechnoSeed[0];
            }

            public TechnoSeed(string name, string type, int year)
            {
                Name = name;
                Type = type;
                EndOfSupport = new DateTime(year, 1, 1);
                Children = new TechnoSeed[0];
            }

            public TechnoSeed[] Children { private get; set; }

            private string Name { get; set; }

            private string Type { get; set; }

            private DateTime? EndOfSupport { get; set; }

            public Technology Save(ISession session)
            {
                var techno = new Technology { Name = Name, EndOfSupport = EndOfSupport };

                if (!string.IsNullOrEmpty(Type))
                {
                    techno.TechnologyType = new DomainValueQuery { Category = DomainValueCategory.TechnologyType, Name = Type }.SingleOrDefault(session);
                }

                session.Save(techno);

                foreach (var child in Children)
                    techno.AddChild(child.Save(session));
                
                return techno;
            }
        }
    }
}
