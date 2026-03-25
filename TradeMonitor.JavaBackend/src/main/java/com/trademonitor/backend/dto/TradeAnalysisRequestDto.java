package com.trademonitor.backend.dto;

public class TradeAnalysisRequestDto {
    private String tradeId;
    private String counterparty;
    private String assetClass;
    private double notional;
    private String region;

    public String getTradeId() {
        return tradeId;
    }

    public void setTradeId(String tradeId) {
        this.tradeId = tradeId;
    }

    public String getCounterparty() {
        return counterparty;
    }

    public void setCounterparty(String counterparty) {
        this.counterparty = counterparty;
    }

    public String getAssetClass() {
        return assetClass;
    }

    public void setAssetClass(String assetClass) {
        this.assetClass = assetClass;
    }

    public double getNotional() {
        return notional;
    }

    public void setNotional(double notional) {
        this.notional = notional;
    }

    public String getRegion() {
        return region;
    }

    public void setRegion(String region) {
        this.region = region;
    }
}