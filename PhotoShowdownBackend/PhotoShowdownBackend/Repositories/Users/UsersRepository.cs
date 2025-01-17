﻿using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Users;

/// <summary>
/// Data Access Layer (DAL) implementation for our Users
/// </summary>
public class UsersRepository: Repository<User>, IUsersRepository
{
    public UsersRepository(PhotoShowdownDbContext _db) : base(_db)
    {
    }
}

