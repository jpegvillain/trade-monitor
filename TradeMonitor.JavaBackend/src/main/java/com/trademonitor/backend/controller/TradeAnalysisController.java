package com.trademonitor.backend.controller;

import com.trademonitor.backend.dto.TradeAnalysisRequestDto;
import com.trademonitor.backend.dto.TradeAnalysisResponseDto;
import org.springframework.web.bind.annotation.*;

import java.util.Random;

@RestController
@RequestMapping("/api/trade-analysis")
@CrossOrigin(origins = "*")
public class TradeAnalysisController {

    private final Random random = new Random();

    @PostMapping
    public TradeAnalysisResponseDto analyseTrade(@RequestBody TradeAnalysisRequestDto request) {
        TradeAnalysisResponseDto response = new TradeAnalysisResponseDto();

        double baseRisk = request.getNotional() > 5_000_000 ? 70.0 : 35.0;
        double assetClassAdjustment = switch (request.getAssetClass()) {
            case "FXO" -> 15.0;
            case "IRS" -> 10.0;
            case "FX" -> 5.0;
            default -> 0.0;
        };

        double regionAdjustment = "Singapore".equalsIgnoreCase(request.getRegion()) ? 5.0 : 0.0;
        double riskScore = Math.min(100.0, baseRisk + assetClassAdjustment + regionAdjustment + random.nextInt(10));
        int latencyMs = 50 + random.nextInt(250);

        String comment;
        if (riskScore >= 80) {
            comment = "High risk trade - urgent review recommended";
        } else if (riskScore >= 50) {
            comment = "Moderate risk trade - monitor closely";
        } else {
            comment = "Low risk trade";
        }

        response.setTradeId(request.getTradeId());
        response.setRiskScore(riskScore);
        response.setLatencyMs(latencyMs);
        response.setComment(comment);

        return response;
    }
}