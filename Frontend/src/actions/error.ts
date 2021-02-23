import { AxiosError } from "axios";
import { Action } from "../reducer";

export const handleAxiosError = (dispatch: React.Dispatch<Action>) => (axiosErrorResponse: AxiosError) => {
    const { response } = axiosErrorResponse;

    const topic = response?.statusText && response?.statusText !== ''
        ? response.statusText
        : response?.status.toString() ?? '';

    let message;

    if(response?.data?.errors){
        message = Object.entries(response!.data.errors).reduce((acc: string, [_, value]) => 
            acc + (value as string[]).reduce((acc, cur) =>
                acc === '' 
                    ? cur
                    : `, ${cur}`
                , '')
            , ''
        );
    }else if(axiosErrorResponse?.message){
        message = axiosErrorResponse?.message;
    }else{
        message = JSON.stringify(response);
    }

    dispatch({
        type: 'messageAdded', 
        message: {
            title: topic,
            content: message,
            type: 'danger'
        }
    });
}