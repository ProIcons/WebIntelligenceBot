using Remora.Results;

namespace WebIntelligence.Common.Models;

public record ServiceError(string Message) : ResultError(Message);