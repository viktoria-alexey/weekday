import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class UtiltityService {

    public static readonly captionAndMessageSeparator = ':';

    public static uniqueId() {
        return this.randomNumber(1000000, 9000000).toString();
    }

    public static randomNumber(min: number, max: number) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    }

    public static splitInTwo(text: string, separator: string): { firstPart: string, secondPart: string } {
        const separatorIndex = text.indexOf(separator);

        if (separatorIndex === -1) {
            return { firstPart: text, secondPart: null };
        }

        const part1 = text.substr(0, separatorIndex).trim();
        const part2 = text.substr(separatorIndex + 1).trim();

        return { firstPart: part1, secondPart: part2 };
    }  
}