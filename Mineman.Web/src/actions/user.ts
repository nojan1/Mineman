import { getToken } from "../auth/token";
import { Action } from "../reducer";

export const populateStateFromProfileIfAvailable = async (dispatch: React.Dispatch<Action>) => {
    if(process.env.REACT_APP_USED_MOCKED_BACKEND === 'true'){
        dispatch({type: 'userLoaded', user: {
            sub: '21312312',
            exp: 999999999999,
            role: [
                'admin'
            ]
        }});
        return;
    }
    const result = await getToken();
    if(result)
        dispatch({type: 'userLoaded', user: result!.decodedToken});
}

const userStartupActions = (dispatch: React.Dispatch<Action>) =>
populateStateFromProfileIfAvailable(dispatch);


export default userStartupActions;