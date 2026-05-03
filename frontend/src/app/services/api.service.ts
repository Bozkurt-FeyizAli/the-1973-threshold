import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface BurdenRequest {
  userBurden: string;
  timestamp: string;
}

export interface TransitionResponseData {
  detectedSentiment: string;
  farewellText: string;
  audioBase64: string;
  imageBase64: string;
  generationTimeMs: number;
}

export interface TransitionResponse {
  status: string;
  data: TransitionResponseData;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  transmitBurden(request: BurdenRequest): Observable<TransitionResponse> {
    return this.http.post<TransitionResponse>(`${this.apiUrl}/Transition/transmit-burden`, request);
  }
}
