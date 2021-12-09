(ns seven-segment-search
  (:require [clojure.string :as string]))

(def file-name "C:\\Users\\david\\source\\repos\\advent-of-code-2021\\input\\day_8_test.txt")
(def input-data (string/split-lines (slurp file-name)))

;; Part One
(defn extract-display-values [input]
  (-> input
      (string/split #"\|")
      (nth 1)
      (string/trim)
      (string/split #" ")))

(defn get-part-one-result [input]
  (->> input
       (map extract-display-values)
       (flatten)
       (map count)
       (map (fn [x] (.contains [2, 3, 4, 7] x)))
       (filter true?)
       (count)))

(def part-one-result (get-part-one-result input-data))
(println part-one-result)