package Parte1;

import java.net.URI;
import java.net.http.HttpRequest;
import com.google.gson.*;

public class Main {
            
    public static void main(String[] args) {
        for (int i = 1; i <= 300; i++)
            {
                String jsonUrl = "https://restcountries.eu/rest/v2/callingcode/" + i;
                boolean hayDatos = true;
                boolean existePais = true;
                HttpRequest request = HttpRequest.newBuilder()
                        .uri(URI.create(jsonUrl))
                        .GET()
                        .build();

                Country pais = new Country();
                try {
                    String json = request.send().body().string();
                    if (json.equals("[]")) {
                        hayDatos = false;
                    } else {
                        pais = new Gson().fromJson(json, Country.class);
                    }
                } catch (Exception e) {
                    e.printStackTrace();
                }
                
                Gestor gestor = new Gestor();
                if (hayDatos) {
                    if (gestor.consultarPais(pais.getCodigoPais())) {
                        existePais = true;
                    }else {
                        existePais = false;
                    }

                    if (!existePais) {
                        gestor.insertarPais(pais);
                    }else{
                        gestor.updatePais(pais);
                    }
                }else{
                    continue;
                } 
            }
    }     
}
