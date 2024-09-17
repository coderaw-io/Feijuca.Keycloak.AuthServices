﻿using TokenManager.Common.Models;

namespace TokenManager.Common.Errors
{
    public static class UserErrors
    {
        public static string TechnicalMessage { get; private set; } = "";

        public static Error TokenGenerationError => new(
            "User.TokenGeneration",
            $"An error occurred while trying to receive information about realm: {TechnicalMessage}"
        );

        public static Error InvalidUserNameOrPasswordError => new(
            "User.InvalidUserNameOrPassword",
            $"An error occurred while trying to get JWT token. Please check username and password. {TechnicalMessage}"
        );

        public static Error InvalidRefreshToken => new(
            "User.InvalidRefreshTokenProvided",
            $"An error occurred while trying to refresh token. {TechnicalMessage}"
        );

        public static Error UserCreationError => new(
            "User.UserCreationError",
            $"An error occurred while trying create a user. {TechnicalMessage}"
        );

        public static Error WrongPasswordDefinition => new(
            "User.WrongPasswordDefinition",
            $"An error occurred while trying to add a new password to the user. {TechnicalMessage}"
        );

        public static Error GetAllUsersError => new(
            "User.GetAllUsersError",
            $"An error occurred while trying get all users. {TechnicalMessage}"
        );

        public static Error DeletionUserError => new(
            "Group.DeletionUserError",
            $"An error occurred while trying delete the user: {TechnicalMessage}"
        );

        public static void SetTechnicalMessage(string technicalMessage)
        {
            TechnicalMessage = technicalMessage;
        }
    }

}
