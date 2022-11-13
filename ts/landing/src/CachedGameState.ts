export class CachedGameState<T> {
    private _nextUpdate: number = 0;
    private _gameState: T | undefined = undefined;
    private readonly _cacheName: string;

    constructor(cacheName: string) {
        this._cacheName = cacheName;

        try {
            const json = localStorage.getItem(this._cacheName);
            if (json !== null)
                this._gameState = JSON.parse(json);
        }
        catch {
            localStorage.removeItem(this._cacheName);
        }
    }

    isExpired(): boolean {
        return this._gameState === undefined || Date.now() > this._nextUpdate;
    }

    set(newState: T) {
        this._gameState = newState;
        localStorage.setItem(this._cacheName, JSON.stringify(newState));
        this.current();
    }

    get(): T | undefined {
        return this._gameState!;
    }

    getRevision(): string | undefined {
        return (this._gameState as any)?.revision;
    }

    clear() {
        this._gameState = undefined;
        this._nextUpdate = 0;
    }

    current() {
        this._nextUpdate = Date.now() + 60 * 60000;
    }
}