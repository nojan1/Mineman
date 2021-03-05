import axios, { AxiosError } from "axios"
import { saveToken } from "../auth/token";

export const login = async (username: string, password: string) => {
    const formData = new FormData();
    formData.append("username", username);
    formData.append("password", password);

    try {
        const response = await axios.post<string>(`${process.env.REACT_APP_BACKEND_URL}/token`, formData);
        saveToken(response.data);
    } catch (error: any) {
        throw error.response.data;
    }
}