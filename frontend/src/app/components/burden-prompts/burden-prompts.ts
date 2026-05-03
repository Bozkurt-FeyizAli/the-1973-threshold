import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

interface PromptSuggestion {
  text: string;
  sentiment: 'NEGATIVE' | 'NEUTRAL' | 'POSITIVE';
  category: string;
}

@Component({
  selector: 'app-burden-prompts',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './burden-prompts.html',
  styleUrl: './burden-prompts.css'
})
export class BurdenPrompts {
  @Output() selectPrompt = new EventEmitter<string>();

  prompts: PromptSuggestion[] = [
    { text: "I still see the faces every time I close my eyes.", sentiment: "NEGATIVE", category: "Weight" },
    { text: "I don't know who I am without this uniform.", sentiment: "NEGATIVE", category: "Identity" },
    { text: "The war ended but it never ended for me.", sentiment: "NEGATIVE", category: "Aftermath" },
    { text: "I lost my brother in a place I can't even pronounce.", sentiment: "NEGATIVE", category: "Loss" },
    { text: "Mama, I'm tired of carrying this anger.", sentiment: "NEUTRAL", category: "Fatigue" },
    { text: "Tell her I'm sorry. I was too young to understand.", sentiment: "NEUTRAL", category: "Regret" },
    { text: "I've been running so long I forgot where home is.", sentiment: "NEUTRAL", category: "Drift" },
    { text: "The sunset reminded me of something I haven't felt in years.", sentiment: "NEUTRAL", category: "Memory" },
    { text: "I'm ready to let the light in again.", sentiment: "POSITIVE", category: "Hope" },
    { text: "Today I heard a song that made me want to live.", sentiment: "POSITIVE", category: "Awakening" },
    { text: "The kids are playing outside. Maybe it's finally quiet.", sentiment: "POSITIVE", category: "Peace" },
    { text: "I forgive you. And I forgive myself.", sentiment: "POSITIVE", category: "Release" },
  ];

  onPromptClick(text: string) {
    this.selectPrompt.emit(text);
  }

  sentimentDot(s: string): string {
    if (s === 'POSITIVE') return '#d97706';
    if (s === 'NEGATIVE') return '#3b82f6';
    return '#78716c';
  }
}
