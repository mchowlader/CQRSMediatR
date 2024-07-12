﻿namespace CQRSMediator.Models;

public class SignUpResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}