package com.trademonitor.backend.dto;

public class TradeAnalysisResponseDto {
    private String tradeId;
    private double riskScore;
    private int latencyMs;
    private String comment;

    public String getTradeId() {
        return tradeId;
    }

    public void setTradeId(String tradeId) {
        this.tradeId = tradeId;
    }

    public double getRiskScore() {
        return riskScore;
    }

    public void setRiskScore(double riskScore) {
        this.riskScore = riskScore;
    }

    public int getLatencyMs() {
        return latencyMs;
    }

    public void setLatencyMs(int latencyMs) {
        this.latencyMs = latencyMs;
    }

    public String getComment() {
        return comment;
    }

    public void setComment(String comment) {
        this.comment = comment;
    }
}