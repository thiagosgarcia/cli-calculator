namespace Domain.ExceptionHandling;

public class MaximumNumbersExceededException(int max, int n) : Exception($"Maximum number of allowed inputs exceeded. Configured: {max} Read: {n}");