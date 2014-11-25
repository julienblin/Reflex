// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISeeder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public interface ISeeder
    {
        int Priority { get; }

        bool IsMinimal { get; }

        void Seed(ISession session);
    }
}
