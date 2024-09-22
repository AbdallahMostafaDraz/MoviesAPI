﻿using Movies.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
    public interface IGenreRepositry : IGenericRepositry<Genre>
    {
       Task<bool> IsValid(byte id);
    }
}
