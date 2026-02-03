namespace Domain.ExceptionHandling;

public class MaximumNumbersExceededException(int max, int n) : Exception($"Maximum number of allowed inputs exceeded. Configured: {max} Read: {n}");
public class NoNegativesAllowedException(long[] negatives) : Exception($"No negative numbers are allowed in the current system. Informed: {string.Join(", ", negatives)}");