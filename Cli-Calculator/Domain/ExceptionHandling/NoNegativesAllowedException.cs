namespace Domain.ExceptionHandling;

public class NoNegativesAllowedException(long[] negatives) : Exception($"No negative numbers are allowed in the current system. Informed: {string.Join(", ", negatives)}");