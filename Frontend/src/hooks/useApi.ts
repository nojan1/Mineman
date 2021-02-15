import { useEffect, useState } from "react";
import { getAuthedAxios } from "../auth";

export type UseApiOptions<T> = {
    defaultValue?: T
    mutateDataFetched?: (data: T) => T
}

function useApi<T = any>(url: string | undefined, options: UseApiOptions<T> | undefined = undefined) {
    const [data, setData] = useState<T | undefined>(options?.defaultValue);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<any>();

    useEffect(() => {
        if (!url)
            return;

        setLoading(true);

        getAuthedAxios().then(axios =>
            axios.get(url)
                .then(x => {
                    const data = options?.mutateDataFetched?.(x.data) ?? x.data;
                    setData(data);
                    setError(null);
                })
                .catch(x => setError(x?.message ?? x))
                .finally(() => setLoading(false))
        );
    }, [url]);

    const post = (body: T) => {
        setLoading(true);

        return getAuthedAxios().then(axios =>
            axios.post(url!, body)
                .then(x => {
                    setError(null);
                    return x.data;
                })
                .catch(x => setError(x))
                .finally(() => setLoading(false))
        );
    }

    return { data, loading, error, post, mutate: setData };
}

export default useApi;