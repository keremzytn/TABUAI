import { UserProfile } from "./user.models";

export interface LoginRequest {
    username: string;
    password: string;
}

export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
    displayName?: string;
}

export interface AuthResponse {
    token: string;
    user: UserProfile;
}
