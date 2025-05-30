﻿namespace ProductManagementApp.API.Services.Interfaces
{
    public interface IUserService
    {
        void CreatePasswordHash(string password, out byte[] hash, out byte[] salt);
        bool VerifyPasswordHash(string password, byte[] hash, byte[] salt);
    }
}
