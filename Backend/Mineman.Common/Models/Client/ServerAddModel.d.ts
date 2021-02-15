declare module server {
	interface serverAddModel {
		description: string;
		worldID: number;
		imageID: number;
		modIDs: number[];
		serverPort: number;
		memoryAllocationMB: number;
	}
}
