namespace WhatIsThisThing.Core.Services;

/// <summary>
/// This class is to help show SQL++ queries in the UI for demo purposes.
/// </summary>
/// <typeparam name="T"></typeparam>
public class WithModalInfo<T>
{
    public string ModalTitle { get; set; }
    public string ModalContent { get; set; }
    public T Data { get; set; }
}