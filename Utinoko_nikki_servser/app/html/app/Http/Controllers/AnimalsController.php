<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;
use Illuminate\Database\QueryException;

class AnimalsController extends Controller
{
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');

        if ($id !== null && $id !== '') {
            $item = DB::table('animals')
                ->select('id', 'name')
                ->where('id', $id)
                ->first();

            return response()->json($item);
        }

        $items = DB::table('animals')
            ->select('id', 'name')
            ->get();

        return response()->json($items);
    }

    //登録
    public function store(Request $request): JsonResponse
    {
        // バリデーション
        $validated = $request->validate([
            'name' => 'required|string|max:255|unique:animals,name'
        ]);

        // データ登録
        $id = DB::table('animals')->insertGetId([
            'name' => $validated['name'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        // 登録した動物を取得
        $animal = DB::table('animals')
            ->select('id', 'name')
            ->where('id', $id)
            ->first();

        return response()->json($animal, 201);
    }

    public function update(Request $request, int $id): JsonResponse
    {
        // バリデーション
        $validated = $request->validate([
            'name' => 'required|string|max:255|unique:animals,name,' . $id,
        ]);

        // 対象の動物が存在するか確認
        $animal = DB::table('animals')
            ->where('id', $id)
            ->first();

        if (!$animal) {
            return response()->json([
                'message' => '指定された動物が存在しません。'
            ], 404);
        }

        // データ更新
        DB::table('animals')
            ->where('id', $id)
            ->update([
                'name' => $validated['name'],
                'updated_at' => now(),
            ]);

        // 更新した動物を取得
        $animal = DB::table('animals')
            ->select('id', 'name')
            ->where('id', $id)
            ->first();

        return response()->json($animal, 200);
    }


    public function destroy(int $id): JsonResponse
    {
        // 動物が存在するか確認
        $animal = DB::table('animals')
            ->where('id', $id)
            ->first();

        if (!$animal) {
            return response()->json([
                'message' => '指定された動物が存在しません。'
            ], 404);
        }

        try {
            // 動物を削除
            DB::table('animals')
                ->where('id', $id)
                ->delete();

            return response()->json([
                'message' => '動物を削除しました。'
            ], 200);

        } catch (QueryException $e) {

            // 外部キー制約違反
            if (($e->errorInfo[1] ?? null) === 1451) {
                return response()->json([
                    'message' => 'この動物は使用中のため削除できません。'
                ], 409);
            }

            // その他のデータベースエラー
            return response()->json([
                'message' => 'データベースエラーが発生しました。'
            ], 500);
        }
    }
}
