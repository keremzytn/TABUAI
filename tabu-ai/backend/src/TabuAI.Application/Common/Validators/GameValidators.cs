using FluentValidation;
using TabuAI.Application.Features.Game.Commands;

namespace TabuAI.Application.Common.Validators;

public class StartGameCommandValidator : AbstractValidator<StartGameCommand>
{
    public StartGameCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanıcı ID boş olamaz.");

        RuleFor(x => x.GameMode)
            .NotEmpty().WithMessage("Oyun modu belirtilmelidir.")
            .Must(mode => new[] { "Solo", "Challenge", "TimeAttack" }.Contains(mode))
            .WithMessage("Geçersiz oyun modu. Solo, Challenge veya TimeAttack olabilir.");

        RuleFor(x => x.Difficulty)
            .InclusiveBetween(1, 4).WithMessage("Zorluk seviyesi 1-4 arasında olmalıdır.")
            .When(x => x.Difficulty.HasValue);
    }
}

public class SubmitPromptCommandValidator : AbstractValidator<SubmitPromptCommand>
{
    public SubmitPromptCommandValidator()
    {
        RuleFor(x => x.GameSessionId)
            .NotEmpty().WithMessage("Oyun oturumu ID boş olamaz.");

        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage("Prompt boş olamaz.")
            .MinimumLength(5).WithMessage("Prompt en az 5 karakter olmalıdır.")
            .MaximumLength(500).WithMessage("Prompt en fazla 500 karakter olabilir.");
    }
}
