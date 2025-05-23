﻿using BarcodeDecodeLib.Models.Dtos.Messages.Auth;

namespace BarcodeDecodeBackend.Services.Interfaces;

public interface IAuthMessageHandler
{
    Task<LoginResult?> Login(LoginDto dto);
}