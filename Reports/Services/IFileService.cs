﻿using System.Threading.Tasks;
using Reports.Entities;

namespace Reports.Services
{
    public interface IFileService
    {
        Task<int> Create(File file);
        Task<File> GetById(int fileId);
        Task Update(File file);
        Task Delete(int fileId);
    }
}
